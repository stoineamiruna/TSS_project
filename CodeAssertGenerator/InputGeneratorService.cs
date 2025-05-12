using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Differencing;
using System;
using System.Collections.Generic;

namespace CodeAssertGenerator
{
    public class InputGeneratorService
    {
        public (string varDeclarations, string inputTrue, string inputFalse) GenerateInputs(ExpressionSyntax condition)
        {
            string varDeclarations = "";
            string inputTrue = "// TODO: generează input true";
            string inputFalse = "// TODO: generează input false";

            if (condition is BinaryExpressionSyntax binaryExpr)
            {
                var left = binaryExpr.Left.ToString();
                var right = binaryExpr.Right.ToString();
                var op = binaryExpr.OperatorToken.ValueText;

                if (int.TryParse(right, out int numericValue))
                {
                    switch (op)
                    {
                        case ">":
                            inputTrue = $"{left} = {numericValue + 1};";
                            inputFalse = $"{left} = {numericValue - 1};";
                            break;
                        case "<":
                            inputTrue = $"{left} = {numericValue - 1};";
                            inputFalse = $"{left} = {numericValue + 1};";
                            break;
                        case "==":
                            inputTrue = $"{left} = {right};";
                            inputFalse = $"{left} = {right} + 1;";
                            break;
                        case "!=":
                            inputTrue = $"{left} = {right} + 1;";
                            inputFalse = $"{left} = {right};";
                            break;
                        case ">=":
                            inputTrue = $"{left} = {numericValue};";
                            inputFalse = $"{left} = {numericValue - 1};";
                            break;
                        case "<=":
                            inputTrue = $"{left} = {numericValue};";
                            inputFalse = $"{left} = {numericValue + 1};";
                            break;
                    }
                }
                else if (right == "null")
                {
                    inputTrue = $"{left} = null;";
                    inputFalse = $"{left} = new object();";
                }
                else if (right.StartsWith("\"")) // string
                {
                    inputTrue = $"{left} = {right};";
                    inputFalse = $"{left} = \"alt\";";
                }

                varDeclarations = $"var {left} = {right};";
            }
            else if (condition is IdentifierNameSyntax identifier)
            {
                var varName = identifier.Identifier.Text;

                // Reguli pe baza numelui variabilei
                if (varName.Contains("activity", StringComparison.OrdinalIgnoreCase))
                {
                    varDeclarations = $"var activity = new object();";
                    inputTrue = "activity = new object();";
                    inputFalse = "activity = null;";
                }
                else if (varName.Contains("id", StringComparison.OrdinalIgnoreCase))
                {
                    varDeclarations = $"var id = null;";
                    inputTrue = "id = null;";
                    inputFalse = "id = new object();";
                }
                else if (varName.Contains("isAdmin", StringComparison.OrdinalIgnoreCase))
                {
                    varDeclarations = $"var isAdmin = true;";
                    inputTrue = "isAdmin = true;";
                    inputFalse = "isAdmin = false;";
                }
            }

            // returnam si declaratiile variabilelor
            return (varDeclarations, inputTrue, inputFalse);
        }
    }
}
