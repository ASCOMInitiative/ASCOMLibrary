using ASCOM.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static SofaWrapperGenerator.Program;
namespace SofaWrapperGenerator
{
    class Program
    {
        static TraceLogger? logger;
        static void Main(string[] args)
        {
            logger = new("WrapperGenerator", true);
            LogMessage("SOFA Wrapper Generator");
            LogMessage("======================\n");

            string inputFile = "Sofa.cs";
            string outputFile = "SofaUpdated.cs";

            // Check if input file exists
            if (!File.Exists(inputFile))
            {
                LogMessage($"Error: {inputFile} not found!");
                LogMessage("Please ensure Sofa.cs is in the same directory as this executable.");
                return;
            }

            try
            {
                var generator = new WrapperGenerator();
                generator.ProcessFile(inputFile, outputFile);

                LogMessage($"\n\nProcessing complete!");
                LogMessage($"Output written to: {outputFile}");
            }
            catch (Exception ex)
            {
                LogMessage($"\nError: {ex.Message}");
                LogMessage(ex.StackTrace);
            }

            LogMessage("\nPress any key to exit...");
            Console.ReadKey();
        }

        internal static void LogMessage(string message)
        {
            Console.WriteLine(message);
            logger?.LogMessage("WrapperGenerator", message);
        }
    }

    class WrapperGenerator
    {
        private readonly List<MethodDefinition> methods = new List<MethodDefinition>();
        private readonly StringBuilder output = new StringBuilder();

        public void ProcessFile(string inputFile, string outputFile)
        {
            LogMessage($"Reading {inputFile}...\n");

            string content = File.ReadAllText(inputFile);

            // Parse the file to extract methods
            ExtractMethods(content);

            LogMessage($"\nFound {methods.Count} methods to process.\n");
            LogMessage("Generating wrappers...\n");

            // Generate the output file
            GenerateOutput(content);

            // Write to file
            File.WriteAllText(outputFile, output.ToString());
        }

        private void ExtractMethods(string content)
        {
            // Pattern to match DllImport declarations with their methods
            var dllImportPattern = @"\[DllImport\([^\]]+EntryPoint\s*=\s*""(iau\w+)""[^\]]*\)\]\s*(?:public\s+static\s+extern\s+)?(\w+)\s+(\w+)\s*\((.*?)\);";

            var matches = Regex.Matches(content, dllImportPattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var method = new MethodDefinition
                {
                    EntryPoint = match.Groups[1].Value,
                    ReturnType = match.Groups[2].Value,
                    MethodName = match.Groups[3].Value,
                    ParametersString = match.Groups[4].Value.Trim()
                };
                LogMessage($"Starting to process method: {method.MethodName} with entry point {method.EntryPoint}");

                // Extract the full declaration including attributes
                int startIndex = match.Index;
                int endIndex = content.IndexOf(");", startIndex) + 2;

                method.XmlDoc = ExtractXmlDoc(content, startIndex);

                // Find the start of the DllImport attribute
                int attributeStart = content.IndexOf("[DllImport", startIndex);
                LogMessage($"Start index: {startIndex}, End index: {endIndex}, Attribute start: {attributeStart}, Content:\r\n{content.Substring(startIndex, endIndex - startIndex)}");

                method.FullDeclaration = content.Substring(attributeStart, endIndex - attributeStart);
                LogMessage($"Full method declaration:\r\n{method.FullDeclaration}");
                // Extract XML documentation if present
                //method.XmlDoc = ExtractXmlDoc(content, attributeStart);
                LogMessage($"XmlDoc: {method.XmlDoc}");

                ParseParameters(method);

                methods.Add(method);

                LogMessage($"Finished processing method: {method.MethodName}\n\n");
            }
        }

        private string ExtractXmlDoc(string content, int methodStartIndex)
        {
            var lines = new List<string>();
            var allLines = content.Substring(0, methodStartIndex).Split('\n');

            for (int i = allLines.Length - 1; i >= 0; i--)
            {
                string line = allLines[i].TrimStart();
                if (line.StartsWith("///"))
                {
                    lines.Insert(0, allLines[i]);
                }
                else if (!string.IsNullOrWhiteSpace(line.Trim()))
                {
                    break;
                }
            }
            string retval = lines.Count > 0 ? string.Join("\n", lines) : "";
            retval = retval.Trim('\r').Trim('\n');
            LogMessage($"Extracted XML Doc: {retval}");
            return retval;
        }

        private void ParseParameters(MethodDefinition method)
        {
            if (string.IsNullOrWhiteSpace(method.ParametersString))
                return;

            // Split by commas, but be careful with nested brackets and generics
            var parameters = SplitParameters(method.ParametersString);

            foreach (var paramStr in parameters)
            {
                var param = new ParameterDefinition
                {
                    FullDeclaration = paramStr.Trim()
                };

                LogMessage($"Parameter FullDeclarationword: {param.FullDeclaration}");


                // Check if it's an array
                param.IsArray = paramStr.Contains("[]");

                // Extract size constraint from MarshalAs attribute
                var sizeMatch = Regex.Match(paramStr, @"SizeConst\s*=\s*(\d+)");
                if (sizeMatch.Success)
                {
                    param.ArraySize = int.Parse(sizeMatch.Groups[1].Value);
                }

                // Check for ref/out modifiers
                param.IsOut = Regex.IsMatch(paramStr, @"\bout\s+");
                param.IsRef = Regex.IsMatch(paramStr, @"\bref\s+");

                // Extract parameter name (last identifier before brackets or end)
                var nameMatch = Regex.Match(paramStr, @"\b(\w+)\s*(?:\[\])?(?:\s|$)");
                if (nameMatch.Success)
                {
                    // Get the last word that's not a keyword
                    var words = Regex.Matches(paramStr, @"\b(\w+)\b");
                    for (int i = words.Count - 1; i >= 0; i--)
                    {
                        string word = words[i].Value;
                        LogMessage($"  Found word: {word}");
                        if (!IsKeyword(word) && !word.Equals("MarshalAs", StringComparison.OrdinalIgnoreCase))
                        {
                            param.Name = word;
                            break;
                        }
                    }
                }

                // Extract the clean type for the public wrapper
                param.PublicType = ExtractPublicType(paramStr);

                method.Parameters.Add(param);
                LogMessage($"    Parameter: {param.Name}, IsArray: {param.IsArray}, ArraySize: {param.ArraySize}, IsOut: {param.IsOut}, IsRef: {param.IsRef}, PublicType: {param.PublicType}");
            }
        }

        private string ExtractPublicType(string paramDeclaration)
        {
            // Remove attributes
            string cleaned = Regex.Replace(paramDeclaration, @"\[.*?\]", "").Trim();
            LogMessage($"ExtractPublicType - Cleaned parameter declaration: {cleaned}");

            cleaned = cleaned.Replace("out ", null);
            cleaned = cleaned.Replace("ref ", null);
            // Extract type (everything before the last identifier)
            var match = Regex.Match(cleaned, @"^(.*?)\s+\w+\s*(?:\[\])?\s*$");
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            LogMessage($"***** ExtractPublicType unable to parse: {paramDeclaration}");
            return "unknown";
        }

        private bool IsKeyword(string word)
        {
            if (word.Contains("Astrom"))
                LogMessage($"***** IsKeyword checking word: {word}");

            var keywords = new HashSet<string>(StringComparer.Ordinal) { "ref", "out", "in", "params", "double", "int", "short", "char", "string", "void", "Astrom", "LdBody" };
            return keywords.Contains(word.ToLower());
        }

        private List<string> SplitParameters(string parametersString)
        {
            // Remove single-line comments (// to end of line)
            string cleanedParameters = RemoveComments(parametersString);

            LogMessage($"Splitting parameters from: {cleanedParameters}");
            var result = new List<string>();
            int bracketDepth = 0;
            int parenDepth = 0;
            var current = new StringBuilder();

            foreach (char c in cleanedParameters)
            {
                if (c == '[') bracketDepth++;
                else if (c == ']') bracketDepth--;
                else if (c == '(') parenDepth++;
                else if (c == ')') parenDepth--;
                else if (c == ',' && bracketDepth == 0 && parenDepth == 0)
                {
                    result.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(c);
            }

            if (current.Length > 0)
            {
                result.Add(current.ToString());
            }

            return result;
        }

        private string RemoveComments(string input)
        {
            var result = new StringBuilder();
            var lines = input.Split('\n');

            foreach (var line in lines)
            {
                // Find single-line comment start
                int commentIndex = line.IndexOf("//");
                if (commentIndex >= 0)
                {
                    // Keep only the part before the comment
                    result.AppendLine(line.Substring(0, commentIndex));
                }
                else
                {
                    result.AppendLine(line);
                }
            }

            return result.ToString();
        }

        private void GenerateOutput(string originalContent)
        {
            // Extract the file header (everything before the first DllImport)
            int firstDllImport = originalContent.IndexOf("[DllImport");
            if (firstDllImport == -1)
            {
                LogMessage("No DllImport declarations found!");
                return;
            }

            // Find the start of the "Sofa entry points" region
            int regionStart = originalContent.IndexOf("#region Sofa entry points");
            if (regionStart == -1)
            {
                regionStart = firstDllImport;
            }

            string header = originalContent.Substring(0, regionStart);

            output.AppendLine(header);

            // Add the validation helper function
            output.AppendLine("#region Private methods");
            output.AppendLine();
            GenerateValidationHelper();
            output.AppendLine("#endregion");
            output.AppendLine();

            output.AppendLine("#region Sofa entry points");
            output.AppendLine();

            // Process each method
            foreach (var method in methods)
            {
                LogMessage($"Processing: {method.MethodName}");
                GenerateMethodWrapper(method);
            }

            output.AppendLine("        #endregion");
            output.AppendLine();
            output.AppendLine("    }");
            output.AppendLine("}");
        }

        private void GenerateValidationHelper()
        {
            output.AppendLine(@"        /// <summary>
        /// Validates that an array parameter is not null and has the expected size.
        /// </summary>
        /// <param name=""array"">The array to validate.</param>
        /// <param name=""expectedSize"">The expected size of the array.</param>
        /// <param name=""paramName"">The name of the parameter being validated.</param>
        /// <exception cref=""ArgumentNullException"">Thrown if the array is null.</exception>
        /// <exception cref=""ArgumentException"">Thrown if the array does not have the expected size.</exception>
        private static void ValidateArray(Array array, int expectedSize, string paramName)
        {
            if (array == null)
            {
                throw new ArgumentNullException(paramName, $""Array {paramName} cannot be null."");
            }

            if (array.Length != expectedSize)
            {
                throw new ArgumentException(
                    $""Array {paramName} must have exactly {expectedSize} elements (length was {array.Length})."",
                    paramName);
            }
        }
");
        }

        private void GenerateMethodWrapper(MethodDefinition method)
        {
            // Generate the private P/Invoke method
            //if (!string.IsNullOrWhiteSpace(method.XmlDoc))
            //{
            //    output.AppendLine(method.XmlDoc.Replace(method.MethodName, $"{method.EntryPoint} (P/Invoke internal)"));
            //}
            //else
            //{
            output.AppendLine($"        /// <summary>");
            output.AppendLine($"        /// {method.MethodName} (P/Invoke the SOFA library).");
            output.AppendLine($"        /// </summary>");
            //}

            // Modify the DllImport declaration to be private
            string privateDeclaration = method.FullDeclaration
                .Replace("public static extern", "private static extern")
                .Replace($"{method.ReturnType} {method.MethodName}", $"{method.ReturnType} {method.EntryPoint}");

            output.AppendLine("        " + privateDeclaration);
            output.AppendLine();




            // Generate the public wrapper method
            if (!string.IsNullOrWhiteSpace(method.XmlDoc))
            {
                output.AppendLine(method.XmlDoc);
            }
            else
            {
                output.AppendLine($"        /// <summary>");
                output.AppendLine($"        /// {method.MethodName} with input validation.");
                output.AppendLine($"        /// </summary>");

                // Add parameter documentation
                foreach (var param in method.Parameters)
                {
                    string description = param.IsArray && param.ArraySize > 0
                        ? $"Array parameter (length {param.ArraySize})."
                        : "Parameter.";
                    output.AppendLine($"        /// <param name=\"{param.Name}\">{description}</param>");
                }
            }

            // Add exception documentation if there are array parameters
            var arrayParams = method.Parameters.Where(p => p.IsArray && !p.IsOut).ToList();
            if (arrayParams.Any())
            {
                output.AppendLine($"        /// <exception cref=\"ArgumentNullException\">Thrown if any array parameter is null.</exception>");
                output.AppendLine($"        /// <exception cref=\"ArgumentException\">Thrown if any array parameter has incorrect length.</exception>");
            }

            if (method.ReturnType != "void")
            {
                output.AppendLine($"        /// <returns>Return value from {method.MethodName}</returns>");
            }

            // Generate method signature
            output.Append($"        public static {method.ReturnType} {method.MethodName}(");

            var publicParams = new List<string>();
            foreach (var param in method.Parameters)
            {
                string modifier = "";
                if (param.IsOut)
                    modifier = "out ";
                else if (param.IsRef)
                    modifier = "ref ";

                if (param.IsArray)
                {
                    LogMessage($"    Parameter {param.Name} is an array of type {param.PublicType} with size {param.ArraySize}");
                    publicParams.Add($"{modifier}{param.PublicType}[] {param.Name}");
                }
                else
                {
                    LogMessage($"    Parameter {param.Name} is of type {param.PublicType}");
                    publicParams.Add($"{modifier}{param.PublicType} {param.Name}");
                }
            }

            output.AppendLine(string.Join(", ", publicParams) + ")");
            output.AppendLine("        {");

            // Generate validation code for array parameters
            foreach (var param in method.Parameters)
            {
                if (param.IsArray && !param.IsOut && param.ArraySize > 0)
                {
                    output.AppendLine($"            ValidateArray({param.Name}, {param.ArraySize}, nameof({param.Name}));");
                }
            }

            if (arrayParams.Any())
            {
                output.AppendLine();
            }

            // Generate the call to the P/Invoke method
            output.Append("            ");
            if (method.ReturnType != "void")
            {
                output.Append("return ");
            }

            output.Append($"{method.EntryPoint}(");

            var callParams = new List<string>();
            foreach (var param in method.Parameters)
            {
                string modifier = "";
                if (param.IsOut) modifier = "out ";
                else if (param.IsRef) modifier = "ref ";

                callParams.Add($"{modifier}{param.Name}");
            }

            output.AppendLine(string.Join(", ", callParams) + ");");

            output.AppendLine("        }");
            output.AppendLine();
        }
    }

    class MethodDefinition
    {
        public string EntryPoint { get; set; }
        public string ReturnType { get; set; }
        public string MethodName { get; set; }
        public string ParametersString { get; set; }
        public string FullDeclaration { get; set; }
        public string XmlDoc { get; set; }
        public List<ParameterDefinition> Parameters { get; set; } = new List<ParameterDefinition>();
    }

    class ParameterDefinition
    {
        public string Name { get; set; }
        public string FullDeclaration { get; set; }
        public string PublicType { get; set; }
        public bool IsArray { get; set; }
        public int ArraySize { get; set; }
        public bool IsOut { get; set; }
        public bool IsRef { get; set; }
    }
}