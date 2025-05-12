using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAssertGenerator
{
    public class ModelAnalyzer
    {
        public class PropertyInfo
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string DefaultValue { get; set; }
            public List<string> Attributes { get; set; } = new List<string>();
            public bool IsCollection { get; set; }
            public string ElementType { get; set; }
        }

        public List<PropertyInfo> ExtractPropertiesFromFile(string filePath)
        {
            var properties = new List<PropertyInfo>();
            var code = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            // Gasim clasa principala
            var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if (classDeclaration == null)
                return properties;

            // Extragem toate proprietatile
            var propertyDeclarations = classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>();
            foreach (var property in propertyDeclarations)
            {
                var propertyName = property.Identifier.ValueText;
               
                if (propertyName.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    continue;
               
                var propertyInfo = new PropertyInfo
                {
                    Name = property.Identifier.ValueText,
                    Type = property.Type.ToString()
                };
                
                if (IsClassType(propertyInfo.Type))
                {
                    continue;
                }

                // Verificam daca este o colectie
                if (propertyInfo.Type.Contains("List<") ||
                    propertyInfo.Type.Contains("IEnumerable<") ||
                    propertyInfo.Type.Contains("ICollection<"))
                {
                    propertyInfo.IsCollection = true;
                    // Extragem tipul elementelor din colectie
                    var startIdx = propertyInfo.Type.IndexOf('<') + 1;
                    var endIdx = propertyInfo.Type.LastIndexOf('>');
                    if (startIdx > 0 && endIdx > startIdx)
                    {
                        var innerType = propertyInfo.Type.Substring(startIdx, endIdx - startIdx);

                        // Setam intotdeauna ca fiind List<innerType>
                        propertyInfo.ElementType = $"List<{innerType}>";
                        propertyInfo.Type = propertyInfo.ElementType;
                        Console.WriteLine(propertyInfo.ElementType);
                        Console.WriteLine(propertyInfo.Type);
                    } 
                }

                // Analizam atributele
                foreach (var attributeList in property.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        propertyInfo.Attributes.Add(attribute.ToString());
                    }
                }

                // Generam valoarea implicita pentru teste
                propertyInfo.DefaultValue = GenerateDefaultValueForType(propertyInfo.Type, propertyInfo.IsCollection, propertyInfo.ElementType);

                properties.Add(propertyInfo);
            }

            return properties;
        }
        private bool IsClassType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return false;

            var normalizedType = type.TrimEnd('?');

            var simpleTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "string", "int", "long", "bool", "decimal", "float", "double", "DateTime", "Guid"
                };

            return !simpleTypes.Contains(normalizedType) &&
                   !normalizedType.StartsWith("IEnumerable", StringComparison.OrdinalIgnoreCase);
        }


        private string GenerateDefaultValueForType(string type, bool isCollection, string elementType)
        {
            if (isCollection)
            {
                return $"new {type}()";
            }

            switch (type.TrimEnd('?').ToLower())
            {
                case "int":
                case "int32":
                    return "42";
                case "long":
                case "int64":
                    return "42L";
                case "decimal":
                    return "42.5m";
                case "double":
                    return "42.5";
                case "float":
                    return "42.5f";
                case "string":
                    return "\"Test String\"";
                case "bool":
                case "boolean":
                    return "true";
                case "datetime":
                    return "new DateTime(2023, 1, 1)";
                case "guid":
                    return "Guid.NewGuid()";
                default:
                    // Aici procesam tipurile complexe (ICollection<T>, List<T>, IEnumerable<T>) fara 'nullable'
                    var cleanedType = type.TrimEnd('?'); // Indepartam '?' pentru a compara corect
                    Console.WriteLine(type);
                    if (cleanedType.StartsWith("ICollection<") || cleanedType.StartsWith("List<") || cleanedType.StartsWith("IEnumerable<"))
                    {
                        // Extragem tipul elementului din colecție
                        var start = cleanedType.IndexOf('<') + 1;
                        var length = cleanedType.IndexOf('>') - start;
                        var elementType2 = cleanedType.Substring(start, length);
                        return $"new List<{elementType2}>()"; // Returnam colectia fara '?' (nullable)
                    }
                    // Daca nu este colectie, returnam tipul simplu
                    return $"new {cleanedType}()";
            }
        }

        public Dictionary<string, List<string>> ExtractAttributesFromFile(string filePath)
        {
            var attributesByProperty = new Dictionary<string, List<string>>();
            var code = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            // Gasim clasa principala
            var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if (classDeclaration == null)
                return attributesByProperty;

            // Extragem toate proprietatile si atributele lor
            var propertyDeclarations = classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>();
            foreach (var property in propertyDeclarations)
            {
                var propertyName = property.Identifier.ValueText;
                var attributes = new List<string>();

                foreach (var attributeList in property.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        attributes.Add(attribute.ToString());
                    }
                }

                attributesByProperty[propertyName] = attributes;
            }

            return attributesByProperty;
        }
    }
}
