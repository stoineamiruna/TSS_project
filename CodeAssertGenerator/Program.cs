using Microsoft.CodeAnalysis.Differencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAssertGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setari de foldere
            string baseDir = AppContext.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..")); // ajungem la folderul soluției
            string sourceFolder = Path.Combine(projectRoot, "Developer-Toolbox"); // codul sursă MVC
            string outputFolder = Path.Combine(projectRoot, "Tests", "CodeAssertGenerated"); // testele generate

            Directory.CreateDirectory(outputFolder);

            var analyzer = new CodeAnalysisService();
            var modelAnalyzer = new ModelAnalyzer();
            var generator = new TestTemplateGenerator();
            var excludedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "badgechallenge",
                "badge",
                "minimumcountattribute",
                "exercise",
                "lockedexercise",
                "errorviewmodel",
                "badgetag",
                "applicationuser",
                "applicationdbcontextmodelsnapshot"
            };

            var allFiles = Directory.GetFiles(sourceFolder, "*.cs", SearchOption.AllDirectories)
            .Where(path => path.Contains(Path.Combine("Models", ""), StringComparison.OrdinalIgnoreCase))
            .ToList();

            var csFiles = allFiles
                .Where(path =>
                {
                    var fileName = Path.GetFileNameWithoutExtension(path).ToLower();
                    return !excludedFiles.Any(excluded => fileName.Contains(excluded));
                })
                .ToList();

            Console.WriteLine($"S-au gasit {csFiles.Count} fisiere in folderul Models.");

            foreach (var file in csFiles)
            {

                var fileName = Path.GetFileNameWithoutExtension(file);
                Console.WriteLine($"Procesarea modelului: {fileName}");

                try
                {
                    // Extragem proprietatile si atributele
                    var properties = modelAnalyzer.ExtractPropertiesFromFile(file);
                    var namespace_ = analyzer.ExtractNamespaceFromFile(file);

                    // Generam codul de test
                    var testCode = generator.GenerateTestClass(fileName, namespace_, properties);

                    // Salvam testul
                    var outPath = Path.Combine(outputFolder, $"{fileName}UnitTest.cs");
                    File.WriteAllText(outPath, testCode);

                    Console.WriteLine($"Generat: {fileName}UnitTest.cs ({properties.Count} proprietati)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Eroare la procesarea {fileName}: {ex.Message}");
                }
            }

            Console.WriteLine("Generare finalizata.");
        }
    }
}