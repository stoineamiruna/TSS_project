using CodeAssertGenerator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Differencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeAssertGenerator
{
    public class TestTemplateGenerator
    {
        public string GenerateTestClass(string className, string namespaceName, List<ModelAnalyzer.PropertyInfo> properties)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine($"using {namespaceName};");
            sb.AppendLine("using Xunit;");
            sb.AppendLine();

            sb.AppendLine($"namespace CodeAssertGenerated");
            sb.AppendLine("{");

            sb.AppendLine($"public class {className}UnitTest");
            sb.AppendLine("{");

            // Constructor test
            sb.AppendLine("    [Fact]");
            sb.AppendLine("    public void Constructor_Test()");
            sb.AppendLine("    {");
            sb.AppendLine("        // Arrange & Act");
            sb.AppendLine($"        var {className.ToLower()} = new {className}();");
            sb.AppendLine("        // Assert");
            sb.AppendLine($"        Assert.NotNull({className.ToLower()});");
            sb.AppendLine($"        Assert.IsType<{className}>({className.ToLower()});");
            sb.AppendLine("    }");
            sb.AppendLine();

            // Properties test
            sb.AppendLine("    [Fact]");
            sb.AppendLine("    public void Properties_InitializedCorrectly()");
            sb.AppendLine("    {");
            sb.AppendLine("        // Arrange");
            sb.AppendLine($"        var {className.ToLower()} = new {className}()");
            sb.AppendLine("        {");

            // Initializare proprietati
            foreach (var prop in properties)
            {
                sb.AppendLine($"            {prop.Name} = {prop.DefaultValue},");
            }
            sb.AppendLine("        };");
            sb.AppendLine();
            sb.AppendLine("        // Act & Assert");

            // Asserts pentru fiecare proprietate
            foreach (var prop in properties)
            {
                if (prop.IsCollection)
                {
                    sb.AppendLine($"        Assert.NotNull({className.ToLower()}.{prop.Name});");
                    sb.AppendLine($"        Assert.Empty({className.ToLower()}.{prop.Name});");
                }
                else
                {
                    sb.AppendLine($"        Assert.Equal({prop.DefaultValue}, {className.ToLower()}.{prop.Name});");
                }
            }
            sb.AppendLine("    }");

            // Adaugam teste pentru atributele de validare
            foreach (var prop in properties)
            {
                var validationAttributes = prop.Attributes
                    .Where(attr => attr.Contains("Required") || attr.Contains("Range") ||
                                  attr.Contains("StringLength") || attr.Contains("MinLength") ||
                                  attr.Contains("MaxLength") || attr.Contains("RegularExpression"));

                if (validationAttributes.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine($"    [Fact]");
                    sb.AppendLine($"    public void {prop.Name}_ValidationAttributes()");
                    sb.AppendLine("    {");

                    foreach (var attr in validationAttributes)
                    {
                        if (attr.Contains("Required"))
                        {
                            sb.AppendLine($"        // Verificam ca validarea Required se aplica");
                            sb.AppendLine($"        var requiredAttribute = typeof({className})");
                            sb.AppendLine($"            .GetProperty(\"{prop.Name}\")");
                            sb.AppendLine($"            .GetCustomAttributes(typeof(RequiredAttribute), false)");
                            sb.AppendLine($"            .FirstOrDefault() as RequiredAttribute;");
                            sb.AppendLine($"        Assert.NotNull(requiredAttribute);");
                        }
                        else if (attr.Contains("StringLength"))
                        {
                            sb.AppendLine($"        // Verificam ca validarea StringLength se aplica");
                            sb.AppendLine($"        var stringLengthAttribute = typeof({className})");
                            sb.AppendLine($"            .GetProperty(\"{prop.Name}\")");
                            sb.AppendLine($"            .GetCustomAttributes(typeof(StringLengthAttribute), false)");
                            sb.AppendLine($"            .FirstOrDefault() as StringLengthAttribute;");
                            sb.AppendLine($"        Assert.NotNull(stringLengthAttribute);");
                            sb.AppendLine($"        Assert.True(stringLengthAttribute.MaximumLength > 0);");
                        }
                        else if (attr.Contains("MaxLength"))
                        {
                            sb.AppendLine($"        // Verificam ca validarea MaxLength se aplica");
                            sb.AppendLine($"        var maxLengthAttribute = typeof({className})");
                            sb.AppendLine($"            .GetProperty(\"{prop.Name}\")");
                            sb.AppendLine($"            .GetCustomAttributes(typeof(MaxLengthAttribute), false)");
                            sb.AppendLine($"            .FirstOrDefault() as MaxLengthAttribute;");
                            sb.AppendLine($"        Assert.NotNull(maxLengthAttribute);");
                            sb.AppendLine($"        Assert.True(maxLengthAttribute.Length > 0);");
                        }

                    }

                    sb.AppendLine("    }");
                }
            }

            sb.AppendLine("}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        
    }
}
