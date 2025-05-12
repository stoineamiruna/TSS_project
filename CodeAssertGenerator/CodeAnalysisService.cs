using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Differencing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace CodeAssertGenerator
{
    public class CodeAnalysisService
    {
        public List<string> ExtractConditionsFromFile(string filePath)
        {
            var code = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();
            var conditions = new List<string>();

            // Cautam toate instructiunile de tip if
            var ifStatements = root.DescendantNodes().OfType<IfStatementSyntax>();
            foreach (var ifStmt in ifStatements)
            {
                var condition = ifStmt.Condition;
                conditions.Add(condition.ToString());
            }

            return conditions;
        }

        public string ExtractNamespaceFromFile(string filePath)
        {
            var code = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            // Cautam declaratia namespace
            var namespaceDecl = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            if (namespaceDecl != null)
            {
                return namespaceDecl.Name.ToString();
            }

            // In caz ca foloseste filescoped namespaces (C# 10+)
            var fileScopedNamespace = root.DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
            if (fileScopedNamespace != null)
            {
                return fileScopedNamespace.Name.ToString();
            }

            return "Developer_Toolbox.Models"; // Default namespace daca nu gasim altul
        }
    }

}