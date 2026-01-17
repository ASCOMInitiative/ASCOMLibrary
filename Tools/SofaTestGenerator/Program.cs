////test\ASCOMStandard.Tests\SOFA\TestGenerator\Program.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SofaTestGenerator
{
    // Generates xUnit tests from the SOFA C test file t_sofa_c.c.
    // - Detects scalar and indexed array assignments (e.g. a = 1.23; arr[0][1] = 2.34;)
    // - Detects iau* calls and maps them to Sofa.* calls
    // - Extracts vvd/viv validations and emits xUnit assertions with precision derived from dval
    // - Handles iauASTROM -> IauAstrom and iauLDBODY -> IauLdBody mapping
    class Program
    {
        const string DefaultCSource = @"t_sofa_c.c";
        const string OutDir = @"..\Generated";
        static readonly Regex FuncStart = new Regex(@"static\s+void\s+(t_[a-z0-9_]+)\s*\(\s*int\s*\*\s*status\s*\)\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex AssignmentScalar = new Regex(@"^\s*([A-Za-z_]\w*)\s*=\s*(.+?);$", RegexOptions.Compiled);
        static readonly Regex AssignmentIndexed2 = new Regex(@"^\s*([A-Za-z_]\w*)\s*\[\s*(\d+)\s*\]\s*\[\s*(\d+)\s*\]\s*=\s*(.+?);$", RegexOptions.Compiled);
        static readonly Regex IauCall = new Regex(@"\biau([A-Za-z0-9_]+)\s*\((.*?)\)\s*;", RegexOptions.Compiled | RegexOptions.Singleline);
        static readonly Regex VvdCall = new Regex(@"\bvvd\s*\(\s*([^\s,]+)\s*,\s*([^\s,]+)\s*,\s*([^\s,]+)\s*,\s*""[^""]*""\s*,\s*""[^""]*""\s*,\s*status\s*\)\s*;", RegexOptions.Compiled);
        static readonly Regex VivCall = new Regex(@"\bviv\s*\(\s*([^\s,]+)\s*,\s*([^\s,]+)\s*,\s*""[^""]*""\s*,\s*""[^""]*""\s*,\s*status\s*\)\s*;", RegexOptions.Compiled);

        static int Main(string[] args)
        {
            var cPath = args.Length > 0 ? args[0] : Path.Combine(Directory.GetCurrentDirectory(), DefaultCSource);
            if (!File.Exists(cPath))
            {
                Console.Error.WriteLine($"C test file not found: {cPath}");
                return 1;
            }

            Directory.CreateDirectory(OutDir);
            var content = File.ReadAllText(cPath);
            var funcMatches = FuncStart.Matches(content).Cast<Match>().ToList();

            // find function ranges by scanning braces from function start
            foreach (Match fm in funcMatches)
            {
                var name = fm.Groups[1].Value; // t_af2a etc
                var startIndex = fm.Index + fm.Length - 1;
                var body = ExtractBlock(content, startIndex);
                if (body == null) continue;

                var testCode = TranslateFunctionToTest(name, body);
                var fileName = Path.Combine(OutDir, $"Sofa_{name}.cs");
                File.WriteAllText(fileName, testCode, Encoding.UTF8);
                Console.WriteLine($"Generated {fileName}");
            }

            Console.WriteLine("Generation complete.");
            return 0;
        }

        // Extracts the {...} block content starting at the brace index (inclusive).
        static string? ExtractBlock(string src, int braceIndex)
        {
            int depth = 0;
            int i = braceIndex;
            if (i >= src.Length || src[i] != '{') return null;
            for (; i < src.Length; i++)
            {
                if (src[i] == '{') { depth++; break; }
            }
            if (depth == 0) return null;
            int start = i;
            i++;
            for (; i < src.Length; i++)
            {
                if (src[i] == '{') depth++;
                else if (src[i] == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        // include braces
                        return src.Substring(start, i - start + 1);
                    }
                }
            }
            return null;
        }

        // Translate a single t_* function body into a C# xUnit test class string.
        static string TranslateFunctionToTest(string tfName, string body)
        {
            var sb = new StringBuilder();
            var className = "Sofa_" + tfName;
            sb.AppendLine("using System;");
            sb.AppendLine("using Xunit;");
            sb.AppendLine("using ASCOM.Tools;");
            sb.AppendLine();
            sb.AppendLine("namespace ASCOMStandard.Tests.SOFA.Generated");
            sb.AppendLine("{");
            sb.AppendLine($"    // Auto-generated from {tfName} in t_sofa_c.c");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");
            sb.AppendLine("        [Fact]");
            sb.AppendLine("        public void Run()");
            sb.AppendLine("        {");

            // Parse indexed assignments into dictionaries for arrays
            var scalarAssignments = new Dictionary<string, string>();
            var indexedAssignments = new Dictionary<string, Dictionary<(int, int), string>>(); // name -> (i,j) -> value
            var singleIndexAssignments = new Dictionary<string, Dictionary<int, string>>(); // name -> i -> value (rare)
            var declaredStructs = new HashSet<string>();

            var lines = SplitStatements(body);
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                // skip comments
                if (line.StartsWith("/*") || line.StartsWith("//")) continue;

                // indexed 2D assignment e.g. ebpv[0][0] =  0.901310875;
                var m2 = AssignmentIndexed2.Match(line);
                if (m2.Success)
                {
                    var name = m2.Groups[1].Value;
                    var i = int.Parse(m2.Groups[2].Value);
                    var j = int.Parse(m2.Groups[3].Value);
                    var val = m2.Groups[4].Value.Trim();
                    if (!indexedAssignments.ContainsKey(name)) indexedAssignments[name] = new Dictionary<(int, int), string>();
                    indexedAssignments[name][(i, j)] = val;
                    continue;
                }

                // scalar assignment e.g. date1 = 2456165.5;
                var ms = AssignmentScalar.Match(line);
                if (ms.Success)
                {
                    var name = ms.Groups[1].Value;
                    var val = ms.Groups[2].Value.Trim();

                    // Avoid capturing function calls left-hand side or pointer declarations
                    // Skip lines that are function prototypes or pointer declarations (contain '(')
                    if (val.Contains("(")) continue;

                    scalarAssignments[name] = val;
                    continue;
                }
            }

            // Emit declarations from gathered data: arrays first
            // For two-dimensional arrays (e.g. ebpv[2][3]) create flattened double[] with row-major order
            foreach (var kv in indexedAssignments)
            {
                var name = kv.Key;
                var cells = kv.Value;
                int maxI = cells.Keys.Max(k => k.Item1);
                int maxJ = cells.Keys.Max(k => k.Item2);
                int rows = maxI + 1;
                int cols = maxJ + 1;
                int len = rows * cols;
                sb.AppendLine($"            // {name}[{rows}][{cols}] flattened row-major");
                sb.AppendLine($"            var {name} = new double[{len}];");
                foreach (var cell in cells.OrderBy(k => k.Key.Item1).ThenBy(k => k.Key.Item2))
                {
                    var (i, j) = cell.Key;
                    var val = cell.Value;
                    int idx = i * cols + j;
                    sb.AppendLine($"            {name}[{idx}] = {NormalizeLiteral(val)};");
                }
                sb.AppendLine();
            }

            // Emit scalars (double/int/char/string) - attempt to guess type from literal
            foreach (var kv in scalarAssignments)
            {
                var name = kv.Key;
                var val = kv.Value;
                // if already declared as array element (skip)
                if (indexedAssignments.ContainsKey(name)) continue;

                // Common special names for structs: astrom, astrom variable of type iauASTROM
                if (name.Equals("astrom", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine($"            var astrom = new IauAstrom();");
                    declaredStructs.Add("astrom");
                    continue;
                }
                if (name.Equals("b", StringComparison.OrdinalIgnoreCase))
                {
                    // will be handled when iauLDBODY b[n] appears
                    continue;
                }

                var literal = NormalizeLiteral(val);
                // Decide type: if literal contains quotes -> string/char, if contains '.' or 'e' -> double else int
                string type = "double";
                if (literal.StartsWith("\"") && literal.EndsWith("\"")) type = "string";
                else if (literal.StartsWith("'") && literal.EndsWith("'")) type = "char";
                else if (!literal.Contains(".") && !Regex.IsMatch(literal, @"[eE]")) type = "int";
                // Emit declaration with initialization
                sb.AppendLine($"            {type} {name} = {literal};");
            }
            sb.AppendLine();

            // Look for iau calls and vvd/viv validations to emit invocation + assertions
            // Process each iau call found in function body (there may be several; handle each)
            var calls = IauCall.Matches(body).Cast<Match>().ToArray();
            // Keep track of variables that are used as out parameters; we must declare them before call
            var outVars = new HashSet<string>();

            foreach (var cm in calls)
            {
                var function = cm.Groups[1].Value; // e.g. Af2a, Anp ...
                var argsText = cm.Groups[2].Value;
                var args = SplitFunctionArgs(argsText).Select(a => a.Trim()).ToList();

                // Pre-declare referenced output variables (those passed as &var)
                foreach (var a in args)
                {
                    var amp = a.Trim();
                    if (amp.StartsWith("&"))
                    {
                        var varName = amp.Substring(1).Trim();
                        if (!scalarAssignments.ContainsKey(varName) && !declaredStructs.Contains(varName))
                        {
                            // We'll attempt to infer type from nearby vvd/viv usage later; for now default to double
                            sb.AppendLine($"            double {varName} = 0.0;");
                            outVars.Add(varName);
                        }
                    }
                    else
                    {
                        // if char pointer? skip
                    }
                }

                // Handle special types in args: arrays (ebpv, ehp), astrom, b (iauLDBODY array)
                var callArgs = new List<string>();
                for (int i = 0; i < args.Count; i++)
                {
                    var a = args[i];
                    if (a.Contains("[")) // passing an array name sometimes appears as 'ebpv' or 'pv' in actual call list; typically it's plain name in the call
                    {
                        // try extract base name
                        var m = Regex.Match(a, @"^([A-Za-z_]\w*)");
                        if (m.Success) callArgs.Add(m.Groups[1].Value);
                        else callArgs.Add(a);
                    }
                    else if (a.Trim().StartsWith("&"))
                    {
                        var varName = a.Trim().Substring(1).Trim();
                        // pass as ref varName
                        callArgs.Add($"ref {varName}");
                    }
                    else if (a.Trim().StartsWith("\"")) // string literal
                    {
                        callArgs.Add(a.Trim());
                    }
                    else if (a.Trim().Equals("astrom", StringComparison.Ordinal))
                    {
                        callArgs.Add("ref astrom");
                    }
                    else if (Regex.IsMatch(a.Trim(), @"^b\s*,?\s*\d")) // pattern not common; fallback
                    {
                        callArgs.Add(a.Trim());
                    }
                    else if (a.Trim().StartsWith("b")) // may be 'b' array when passing iauLDBODY b
                    {
                        callArgs.Add("b");
                    }
                    else if (indexedAssignments.ContainsKey(a.Trim()))
                    {
                        callArgs.Add(a.Trim());
                    }
                    else
                    {
                        // numeric literal or variable; normalize (e.g. 2456165.5)
                        callArgs.Add(NormalizeLiteral(a.Trim()));
                    }
                }

                // Special handling: if function uses iauLDBODY array b[...] assigned with b[i].pv[...] pattern
                // Detect b[...] assignments in the body and emit corresponding managed IauLdBody[] b = new IauLdBody[n] and field initializers.
                if (body.Contains("b["))
                {
                    // parse b[i].bm = ... ; b[i].dl = ... ; b[i].pv[ri][ci] = ...
                    var bCells = new Dictionary<int, Dictionary<string, object>>();
                    var bLines = SplitStatements(body);
                    foreach (var line in bLines)
                    {
                        var l = line.Trim();
                        var mBm = Regex.Match(l, @"b\[(\d+)\]\.bm\s*=\s*(.+?);");
                        if (mBm.Success)
                        {
                            int idx = int.Parse(mBm.Groups[1].Value);
                            var val = NormalizeLiteral(mBm.Groups[2].Value.Trim());
                            if (!bCells.ContainsKey(idx)) bCells[idx] = new Dictionary<string, object>();
                            bCells[idx]["bm"] = val;
                            continue;
                        }
                        var mDl = Regex.Match(l, @"b\[(\d+)\]\.dl\s*=\s*(.+?);");
                        if (mDl.Success)
                        {
                            int idx = int.Parse(mDl.Groups[1].Value);
                            var val = NormalizeLiteral(mDl.Groups[2].Value.Trim());
                            if (!bCells.ContainsKey(idx)) bCells[idx] = new Dictionary<string, object>();
                            bCells[idx]["dl"] = val;
                            continue;
                        }
                        var mPv = Regex.Match(l, @"b\[(\d+)\]\.pv\[(\d+)\]\[(\d+)\]\s*=\s*(.+?);");
                        if (mPv.Success)
                        {
                            int idx = int.Parse(mPv.Groups[1].Value);
                            int r = int.Parse(mPv.Groups[2].Value);
                            int c = int.Parse(mPv.Groups[3].Value);
                            var val = NormalizeLiteral(mPv.Groups[4].Value.Trim());
                            if (!bCells.ContainsKey(idx)) bCells[idx] = new Dictionary<string, object>();
                            if (!bCells[idx].ContainsKey("pv")) bCells[idx]["pv"] = new Dictionary<(int, int), string>();
                            var dict = (Dictionary<(int, int), string>)bCells[idx]["pv"];
                            dict[(r, c)] = val;
                            continue;
                        }
                    }

                    if (bCells.Count > 0)
                    {
                        int n = bCells.Keys.Max() + 1;
                        sb.AppendLine($"            var b = new IauLdBody[{n}];");
                        for (int i = 0; i < n; i++)
                        {
                            sb.AppendLine($"            b[{i}] = new IauLdBody();");
                            if (bCells.ContainsKey(i))
                            {
                                var cell = bCells[i];
                                if (cell.TryGetValue("bm", out var bmVal))
                                {
                                    sb.AppendLine($"            b[{i}].bm = {bmVal};");
                                }
                                if (cell.TryGetValue("dl", out var dlVal))
                                {
                                    sb.AppendLine($"            b[{i}].dl = {dlVal};");
                                }
                                if (cell.TryGetValue("pv", out var pvMapObj))
                                {
                                    var pvMap = (Dictionary<(int, int), string>)pvMapObj;
                                    // assume pv is 2x3 -> flattened length 6 row major
                                    sb.AppendLine($"            b[{i}].pv = new double[6];");
                                    int maxR = pvMap.Keys.Max(k => k.Item1);
                                    int maxC = pvMap.Keys.Max(k => k.Item2);
                                    int cols = Math.Max(3, maxC + 1);
                                    foreach (var kvp in pvMap)
                                    {
                                        var (r, c) = kvp.Key;
                                        var val = kvp.Value;
                                        var idx = r * cols + c;
                                        sb.AppendLine($"            b[{i}].pv[{idx}] = {val};");
                                    }
                                }
                            }
                        }
                        sb.AppendLine();
                    }
                }

                // Emit invoke call mapping iauXYZ -> Sofa.Xyz (capitalize first letter)
                var sofaName = function; // often same case as DllImport entry used in Sofa.cs earlier
                                         // Map iauEpj -> Epj etc (names preserved)
                                         // Build invocation string
                var invocation = $"Sofa.{sofaName}({string.Join(", ", callArgs)})";
                // Some functions return int/short/double; if last arg contained &out, call used ref. If return ignored in C, we may capture it for asserts if vvd/viv reference it.
                // We'll capture the return if validation references a variable set to the result (e.g., j = iauAf2a(...);)
                // But our parsing does not know the left-hand assignment. Try to detect if preceding code had "j = iau..." pattern
                var leftAssignMatch = Regex.Match(body, $@"(\b[A-Za-z_]\w*)\s*=\s*iau{Regex.Escape(function)}\s*\(", RegexOptions.IgnoreCase);
                if (leftAssignMatch.Success)
                {
                    var leftVar = leftAssignMatch.Groups[1].Value;
                    // declare left variable if not present
                    if (!scalarAssignments.ContainsKey(leftVar) && !outVars.Contains(leftVar))
                    {
                        sb.AppendLine($"            int {leftVar};");
                    }
                    sb.AppendLine($"            {leftVar} = (int){invocation};");
                }
                else
                {
                    // For functions returning double (no assignment), capture into var if validation references it (vvd uses function call directly sometimes)
                    // If invocation was used directly in vvd(iauAnp(-0.1), ... ), we will not pre-invoke; instead assertions below will call Sofa.* inline.
                    sb.AppendLine($"            // call: {invocation};");
                }
                sb.AppendLine();
            }

            // Process vvd/viv assertions in order they appear in the C body
            var vvdMatches = VvdCall.Matches(body).Cast<Match>().ToList();
            var vivMatches = VivCall.Matches(body).Cast<Match>().ToList();

            // Interleave original ordering by searching for positions
            var validations = new List<(int pos, string code)>();
            foreach (Match m in VvdCall.Matches(body))
            {
                var pos = m.Index;
                var actual = m.Groups[1].Value.Trim();
                var expected = m.Groups[2].Value.Trim();
                var dval = m.Groups[3].Value.Trim();
                int precision = DvalToPrecision(dval);
                // If actual is a function call (iauAnp(-0.1)), translate to Sofa call inline
                var actualExpr = actual;
                var iauInline = Regex.Match(actual, @"iau([A-Za-z0-9_]+)\s*\((.*)\)");
                if (iauInline.Success)
                {
                    var fn = iauInline.Groups[1].Value;
                    var argsRaw = iauInline.Groups[2].Value;
                    var args = SplitFunctionArgs(argsRaw).Select(a => NormalizeLiteral(a.Trim()));
                    actualExpr = $"Sofa.{fn}({string.Join(", ", args)})";
                    // expected is numeric literal
                    var expectedLiteral = NormalizeLiteral(expected);
                    validations.Add((pos, $"            Assert.Equal({expectedLiteral}, {actualExpr}, {precision});"));
                }
                else
                {
                    // actual is a variable
                    var expectedLiteral = NormalizeLiteral(expected);
                    validations.Add((pos, $"            Assert.Equal({expectedLiteral}, {actual}, {precision});"));
                }
            }

            foreach (Match m in VivCall.Matches(body))
            {
                var pos = m.Index;
                var actual = m.Groups[1].Value.Trim();
                var expected = m.Groups[2].Value.Trim();
                // integer compare
                validations.Add((pos, $"            Assert.Equal({expected}, {actual});"));
            }

            // sort validations by original position
            foreach (var v in validations.OrderBy(v => v.pos))
            {
                sb.AppendLine(v.code);
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        // Splits function/block text into semicolon-terminated statements (keeps whole statement text)
        static List<string> SplitStatements(string block)
        {
            var list = new List<string>();
            var sb = new StringBuilder();
            int parenDepth = 0;
            int braceDepth = 0;
            bool inString = false;
            char stringChar = '\0';
            for (int i = 0; i < block.Length; i++)
            {
                var ch = block[i];
                sb.Append(ch);
                if (inString)
                {
                    if (ch == '\\') { i++; if (i < block.Length) sb.Append(block[i]); continue; }
                    if (ch == stringChar) inString = false;
                }
                else
                {
                    if (ch == '"' || ch == '\'') { inString = true; stringChar = ch; }
                    else if (ch == '(') parenDepth++;
                    else if (ch == ')') parenDepth = Math.Max(0, parenDepth - 1);
                    else if (ch == '{') braceDepth++;
                    else if (ch == '}') braceDepth = Math.Max(0, braceDepth - 1);
                    else if (ch == ';' && parenDepth == 0 && braceDepth == 0)
                    {
                        list.Add(sb.ToString());
                        sb.Clear();
                    }
                }
            }
            if (sb.Length > 0) list.Add(sb.ToString());
            return list;
        }

        // Split function arg list (handles nested parentheses, strings)
        static List<string> SplitFunctionArgs(string args)
        {
            var list = new List<string>();
            var sb = new StringBuilder();
            int depth = 0;
            bool inString = false;
            char sc = '\0';
            for (int i = 0; i < args.Length; i++)
            {
                var ch = args[i];
                if (inString)
                {
                    sb.Append(ch);
                    if (ch == '\\') { i++; if (i < args.Length) sb.Append(args[i]); continue; }
                    if (ch == sc) inString = false;
                    continue;
                }
                if (ch == '"' || ch == '\'')
                {
                    inString = true; sc = ch; sb.Append(ch); continue;
                }
                if (ch == '(') { depth++; sb.Append(ch); continue; }
                if (ch == ')') { depth--; sb.Append(ch); continue; }
                if (ch == ',' && depth == 0)
                {
                    list.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }
                sb.Append(ch);
            }
            if (sb.Length > 0) list.Add(sb.ToString());
            return list.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        }

        // Normalize numeric literals and C-style floats to C#-compatible form.
        static string NormalizeLiteral(string lit)
        {
            lit = lit.Trim();
            // replace C exponent forms like 1e-12 (already ok)
            // replace C constants like 3e-10 -> 3e-10
            // replace integer '0' stays
            // handle suffixes like 'e' etc (rare)
            // map C char constants e.g. '-' -> '\'-\'' or C string to C# string
            if (lit.StartsWith("\"") && lit.EndsWith("\"")) return lit;
            if (lit.StartsWith("'") && lit.EndsWith("'")) return lit; // char literal ok
                                                                      // handle scientific notation with spaces or missing leading 0
            lit = lit.Replace("D", "E").Replace("d", "e");
            // remove casts like (double)123 -> 123
            lit = Regex.Replace(lit, @"\([A-Za-z_][A-Za-z0-9_]*\)\s*", "");
            return lit;
        }

        // Convert dval like 1e-12 to precision digits (approx)
        static int DvalToPrecision(string dval)
        {
            dval = dval.Trim();
            double dv;
            if (!double.TryParse(dval, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out dv))
            {
                // default precision
                return 12;
            }
            if (dv <= 0.0) return 0;
            var prec = (int)Math.Round(-Math.Log10(dv));
            if (prec < 0) prec = 0;
            return prec;
        }
    }
}