using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

/// <summary>
/// SOFA Test Generator - Parses t_sofa_c.c and generates C# test files
/// </summary>
class SofaTestGenerator
{
    private static readonly string OUTPUT_DIRECTORY = @"test\ASCOMStandard.Tests\SOFA";
    private static readonly string NAMESPACE = "SOFA.Generated";

    static void Main(string[] args)
    {
        string sofaTestFile = args.Length > 0 ? args[0] : "t_sofa_c.c";
        
        if (!File.Exists(sofaTestFile))
        {
            Console.WriteLine($"Error: File not found: {sofaTestFile}");
            return;
        }

        string content = File.ReadAllText(sofaTestFile);
        
        // Extract all test functions
        var testFunctions = ExtractTestFunctions(content);
        
        Console.WriteLine($"Found {testFunctions.Count} test functions");
        
        // Generate C# test files
        foreach (var testFunction in testFunctions)
        {
            string csharpCode = GenerateCSharpTest(testFunction);
            string fileName = GenerateFileName(testFunction);
            string filePath = Path.Combine(OUTPUT_DIRECTORY, fileName);
            
            // Create directory if it doesn't exist
            Directory.CreateDirectory(OUTPUT_DIRECTORY);
            
            try
            {
                File.WriteAllText(filePath, csharpCode);
                Console.WriteLine($"Generated: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating {fileName}: {ex.Message}");
            }
        }
        
        Console.WriteLine("Generation complete!");
    }

    private static List<TestFunction> ExtractTestFunctions(string content)
    {
        var testFunctions = new List<TestFunction>();

        // Pattern to match test functions: static void t_something(int *status) e.g. static void t_a2tf(int *status)
        //var pattern = @"static\s+void\s+(t_\w+)\s*\(\s*int\s*\*status\s*\)\s*\{([^}]+(?:\{[^}]*\}[^}]*)*)\}";

        var pattern = @"static\s+void\s+(t_\w+)\s*\(\s*int\s*\*status\s*\)";
        var matches = Regex.Matches(content, pattern, RegexOptions.Multiline);
        
        foreach (Match match in matches)
        {
            string functionName = match.Groups[1].Value;
            string functionBody = match.Groups[2].Value;
            
            testFunctions.Add(new TestFunction
            {
                CName = functionName,
                Body = functionBody.Trim()
            });
        }
        
        return testFunctions;
    }

    private static string GenerateFileName(TestFunction testFunction)
    {
        // Convert t_something to Sofa_t_something.cs
        string baseName = testFunction.CName;
        return $"Sofa_{baseName}.cs";
    }

    private static string GenerateCSharpTest(TestFunction testFunction)
    {
        string methodName = ConvertCNameToMethodName(testFunction.CName);
        string csharpBody = ConvertCBodyToCSharp(testFunction.Body, methodName);
        
        var sb = new StringBuilder();
        sb.AppendLine($"//test\\ASCOMStandard.Tests\\SOFA\\Sofa_{testFunction.CName}.cs");
        sb.AppendLine("using Xunit;");
        sb.AppendLine("using ASCOM.Tools;");
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"namespace {NAMESPACE}");
        sb.AppendLine("{");
        sb.AppendLine($"    public class Sofa_{testFunction.CName}");
        sb.AppendLine("    {");
        sb.AppendLine("        [Fact]");
        sb.AppendLine($"        public void {methodName}()");
        sb.AppendLine("        {");
        sb.AppendLine(csharpBody);
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }

    private static string ConvertCNameToMethodName(string cName)
    {
        // Convert t_something to Something (PascalCase)
        if (cName.StartsWith("t_"))
        {
            cName = cName.Substring(2);
        }
        
        // Convert snake_case to PascalCase
        var parts = cName.Split('_');
        var sb = new StringBuilder();
        foreach (var part in parts)
        {
            if (part.Length > 0)
            {
                sb.Append(char.ToUpper(part[0]));
                if (part.Length > 1)
                {
                    sb.Append(part.Substring(1));
                }
            }
        }
        
        return sb.ToString();
    }

    private static string ConvertCBodyToCSharp(string cBody, string methodName)
    {
        var lines = cBody.Split('\n');
        var sb = new StringBuilder();
        
        foreach (var line in lines)
        {
            string trimmed = line.Trim();
            
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("//"))
                continue;
            
            // Skip variable declarations that start with iauXXX struct types
            if (trimmed.StartsWith("iauASTROM") || trimmed.StartsWith("iauLDBODY"))
                continue;
            
            // Convert declarations
            string converted = ConvertCDeclarations(trimmed);
            
            // Convert function calls
            converted = ConvertCFunctionCalls(converted);
            
            // Convert assertions
            converted = ConvertCAssertions(converted);
            
            if (!string.IsNullOrWhiteSpace(converted))
            {
                sb.AppendLine("            " + converted);
            }
        }
        
        return sb.ToString();
    }

    private static string ConvertCDeclarations(string cLine)
    {
        // Replace C variable declarations with C# equivalents
        cLine = Regex.Replace(cLine, @"double\s+(\w+)(?:\s*=\s*([^;,]+))?", m =>
        {
            string varName = m.Groups[1].Value;
            string value = m.Groups[2].Value.Trim();
            if (string.IsNullOrEmpty(value))
                return $"double {varName} = 0;";
            return $"double {varName} = {value};";
        });
        
        cLine = Regex.Replace(cLine, @"int\s+(\w+)(?:\s*=\s*([^;,]+))?", m =>
        {
            string varName = m.Groups[1].Value;
            string value = m.Groups[2].Value.Trim();
            if (string.IsNullOrEmpty(value))
                return $"int {varName} = 0;";
            return $"int {varName} = {value};";
        });
        
        cLine = Regex.Replace(cLine, @"char\s+(\w+)(?:\s*=\s*([^;,]+))?", m =>
        {
            string varName = m.Groups[1].Value;
            string value = m.Groups[2].Value.Trim();
            if (string.IsNullOrEmpty(value))
                return $"char {varName} = ' ';";
            return $"char {varName} = {value};";
        });
        
        // Array declarations
        cLine = Regex.Replace(cLine, @"(\w+)\s+(\w+)\[(\d+)\]", m =>
        {
            string type = m.Groups[1].Value;
            string varName = m.Groups[2].Value;
            string size = m.Groups[3].Value;
            
            if (type == "int")
                return $"int[] {varName} = new int[{size}];";
            else if (type == "double")
                return $"double[] {varName} = new double[{size}];";
            
            return m.Value;
        });
        
        // 2D array declarations
        cLine = Regex.Replace(cLine, @"(\w+)\s+(\w+)\[(\d+)\]\[(\d+)\]", m =>
        {
            string type = m.Groups[1].Value;
            string varName = m.Groups[2].Value;
            
            if (type == "int")
                return $"int[][] {varName} = new int[{m.Groups[3].Value}][];";
            else if (type == "double")
                return $"double[][] {varName} = new double[{m.Groups[3].Value}][];";
            
            return m.Value;
        });
        
        return cLine;
    }

    private static string ConvertCFunctionCalls(string cLine)
    {
        // Replace function names: iauXxxx -> Sofa.Xxxx
        cLine = Regex.Replace(cLine, @"\biau(\w+)\s*\(", m =>
        {
            string funcName = m.Groups[1].Value;
            return $"Sofa.{funcName}(";
        });
        
        // Handle vvd and viv calls - these are validation functions, convert to Assert
        // Pattern: vvd(value, expected, tolerance, "function", "test", status);
        cLine = Regex.Replace(cLine, @"vvd\s*\(\s*([^,]+),\s*([^,]+),\s*([^,]+),", m =>
        {
            string value = m.Groups[1].Value.Trim();
            string expected = m.Groups[2].Value.Trim();
            string tolerance = m.Groups[3].Value.Trim();
            
            // Extract precision from tolerance (e.g., 1e-12 -> 12)
            int precision = ExtractPrecision(tolerance);
            return $"Assert.Equal({expected}, {value}, {precision});";
        });
        
        // Handle viv calls
        cLine = Regex.Replace(cLine, @"viv\s*\(\s*\(\s*int\s*\)\s*(\w+),\s*([^,]+),", m =>
        {
            string value = m.Groups[1].Value.Trim();
            string expected = m.Groups[2].Value.Trim();
            return $"Assert.Equal({expected}, (int){value});";
        });
        
        cLine = Regex.Replace(cLine, @"viv\s*\(\s*(\w+),\s*([^,]+),", m =>
        {
            string value = m.Groups[1].Value.Trim();
            string expected = m.Groups[2].Value.Trim();
            return $"Assert.Equal({expected}, {value});";
        });
        
        // Clean up any remaining validation function calls
        cLine = Regex.Replace(cLine, @",\s*""[^""]*"",\s*""[^""]*"",\s*status\s*\)\s*;", ");");
        cLine = Regex.Replace(cLine, @",\s*""[^""]*"",\s*status\s*\)\s*;", ");");
        
        return cLine;
    }

    private static string ConvertCAssertions(string cLine)
    {
        // Already handled in ConvertCFunctionCalls
        return cLine;
    }

    private static int ExtractPrecision(string tolerance)
    {
        // Convert 1e-12 to 12
        var match = Regex.Match(tolerance, @"e-(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int precision))
        {
            return precision;
        }
        return 12; // Default precision
    }
}

class TestFunction
{
    public string CName { get; set; }
    public string Body { get; set; }
}