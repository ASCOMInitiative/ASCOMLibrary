using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MethodSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: MethodSorter <filepath>");
                Console.WriteLine("Example: MethodSorter Tests.cs");
                return;
            }

            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            try
            {
                string sourceCode = File.ReadAllText(filePath);
                string sortedCode = SortMethods(sourceCode);
                
                // Create backup
                string backupPath = filePath + ".backup";
                File.Copy(filePath, backupPath, true);
                Console.WriteLine($"Backup created: {backupPath}");
                
                // Write sorted code
                File.WriteAllText(filePath, sortedCode);
                Console.WriteLine($"Methods sorted successfully in: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static string SortMethods(string sourceCode)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            var rewriter = new MethodSorterRewriter();
            SyntaxNode newRoot = rewriter.Visit(root);

            return newRoot.ToFullString();
        }
    }

    class MethodSorterRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Get all members
            var members = node.Members.ToList();
            
            // Separate methods from other members
            var methods = members.OfType<MethodDeclarationSyntax>().ToList();
            var nonMethods = members.Where(m => !(m is MethodDeclarationSyntax)).ToList();
            
            if (methods.Count == 0)
            {
                return base.VisitClassDeclaration(node);
            }

            // Group members by type to preserve structure
            var groupedMembers = new List<SyntaxNode>();
            
            // Add fields and properties first (preserve original order)
            var fieldsAndProperties = members.Where(m => 
                m is FieldDeclarationSyntax || 
                m is PropertyDeclarationSyntax).ToList();
            groupedMembers.AddRange(fieldsAndProperties);

            // Add constructor(s)
            var constructors = members.OfType<ConstructorDeclarationSyntax>().ToList();
            groupedMembers.AddRange(constructors);

            // Sort and add methods alphabetically
            var sortedMethods = methods
                .OrderBy(m => m.Identifier.Text)
                .ToList();
            groupedMembers.AddRange(sortedMethods);

            // Add any other members
            var otherMembers = members.Where(m => 
                !(m is FieldDeclarationSyntax) &&
                !(m is PropertyDeclarationSyntax) &&
                !(m is ConstructorDeclarationSyntax) &&
                !(m is MethodDeclarationSyntax)).ToList();
            groupedMembers.AddRange(otherMembers);

            // Create new class with sorted members
            var newNode = node.WithMembers(
                (SyntaxList<MemberDeclarationSyntax>)SyntaxFactory.List(groupedMembers));

            return base.VisitClassDeclaration(newNode);
        }
    }
}