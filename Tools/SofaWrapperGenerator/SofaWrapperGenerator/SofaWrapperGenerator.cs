using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SofaWrapperGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SOFA Wrapper Generator");
            Console.WriteLine("======================\n");

            string inputFile = "Sofa.cs";
            string outputFile = "SofaUpdated.cs";

            // Check if input file exists
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: {inputFile} not found!");
                Console.WriteLine("Please ensure Sofa.cs is in the same directory as this executable.");
                return;
            }

            try
            {
                var generator = new WrapperGenerator();
                generator.ProcessFile(inputFile, outputFile);

                Console.WriteLine($"\n\nProcessing complete!");
                Console.WriteLine($"Output written to: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    class WrapperGenerator
    {
        private readonly List<MethodDefinition> methods = new List<MethodDefinition>();
        private readonly StringBuilder output = new StringBuilder();

        public void ProcessFile(string inputFile, string outputFile)
        {
            Console.WriteLine($"Reading {inputFile}...\n");

            string content = File.ReadAllText(inputFile);

            // Parse the file to extract methods
            ExtractMethods(content);

            Console.WriteLine($"\nFound {methods.Count} methods to process.\n");
            Console.WriteLine("Generating wrappers...\n");

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
                Console.WriteLine($"Extracted method: {method.MethodName} with entry point {method.EntryPoint}");

                // Extract the full declaration including attributes
                int startIndex = match.Index;
                int endIndex = content.IndexOf(");", startIndex) + 2;

                // Find the start of the DllImport attribute
                int attributeStart = content.IndexOf("[DllImport", startIndex);
                Console.WriteLine($"Start index: {startIndex}, End index: {endIndex}, Attribute start: {attributeStart}, Content: {content.Substring(startIndex,endIndex-startIndex)}");

                method.FullDeclaration = content.Substring(attributeStart, endIndex - attributeStart);
                Console.WriteLine($"Full declaration: {method.FullDeclaration}");
                // Extract XML documentation if present
                method.XmlDoc = ExtractXmlDoc(content, attributeStart);

                ParseParameters(method);

                methods.Add(method);
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
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
            }

            return lines.Count > 0 ? string.Join("\n", lines) : "";
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
            }
        }

        private string ExtractPublicType(string paramDeclaration)
        {
            // Remove attributes
            string cleaned = Regex.Replace(paramDeclaration, @"\[.*?\]", "").Trim();

            // Extract type (everything before the last identifier)
            var match = Regex.Match(cleaned, @"^(.*?)\s+\w+\s*(?:\[\])?\s*$");
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return "unknown";
        }

        private bool IsKeyword(string word)
        {
            var keywords = new HashSet<string> { "ref", "out", "in", "params", "double", "int", "short", "char", "string", "void" };
            return keywords.Contains(word.ToLower());
        }

        private List<string> SplitParameters(string parametersString)
        {
            var result = new List<string>();
            int bracketDepth = 0;
            int parenDepth = 0;
            var current = new StringBuilder();

            foreach (char c in parametersString)
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

        private void GenerateOutput(string originalContent)
        {
            // Extract the file header (everything before the first DllImport)
            int firstDllImport = originalContent.IndexOf("[DllImport");
            if (firstDllImport == -1)
            {
                Console.WriteLine("No DllImport declarations found!");
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
            output.AppendLine("#region Sofa entry points");
            output.AppendLine();

            // Add the validation helper function
            GenerateValidationHelper();

            // Process each method
            foreach (var method in methods)
            {
                Console.WriteLine($"Processing: {method.MethodName}");
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
            if (!string.IsNullOrWhiteSpace(method.XmlDoc))
            {
                output.AppendLine(method.XmlDoc.Replace(method.MethodName, $"{method.EntryPoint} (P/Invoke internal)"));
            }
            else
            {
                output.AppendLine($"        /// <summary>");
                output.AppendLine($"        /// {method.MethodName} (P/Invoke internal).");
                output.AppendLine($"        /// </summary>");
            }

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
            }

            // Add parameter documentation
            foreach (var param in method.Parameters)
            {
                string description = param.IsArray && param.ArraySize > 0
                    ? $"Array parameter (length {param.ArraySize})."
                    : "Parameter.";
                output.AppendLine($"        /// <param name=\"{param.Name}\">{description}</param>");
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
                if (param.IsOut) modifier = "out ";
                else if (param.IsRef) modifier = "ref ";

                publicParams.Add($"{modifier}{param.PublicType} {param.Name}");
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