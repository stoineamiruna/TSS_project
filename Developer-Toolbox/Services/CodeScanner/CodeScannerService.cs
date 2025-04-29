using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Developer_Toolbox.Services.CodeScanner;
using Developer_Toolbox.Services.Vulnerability;
using Microsoft.Extensions.Configuration;

namespace YourApp.Services.CodeScanner
{
    public class CodeScannerService : ICodeScannerService
    {
        private readonly IVulnerabilityService _vulnerabilityService;
        private readonly IConfiguration _configuration;
        private readonly string _projectRootPath;

        public CodeScannerService(IVulnerabilityService vulnerabilityService, IConfiguration configuration)
        {
            _vulnerabilityService = vulnerabilityService;
            _configuration = configuration;

            // Get the project root directory from configuration or use current directory
            _projectRootPath = _configuration["ProjectScan:RootDirectory"] ??
                             Directory.GetCurrentDirectory();
        }

        public async Task<ProjectVulnerabilityReport> ScanProjectAsync(string projectDirectory = null)
        {
            // Use provided directory or default to project root
            string dirToScan = projectDirectory ?? _projectRootPath;

            // Find all .cs files in the directory and subdirectories
            var csharpFiles = Directory.GetFiles(dirToScan, "*.cs", SearchOption.AllDirectories);

            var report = new ProjectVulnerabilityReport
            {
                TotalFilesScanned = csharpFiles.Length,
                VulnerabilitiesByRiskLevel = new Dictionary<string, int>
                {
                    { "Critic", 0 },
                    { "Ridicat", 0 },
                    { "Mediu", 0 },
                    { "Scazut", 0 }
                }
            };

            // Process each file
            foreach (var file in csharpFiles)
            {
                try
                {
                    var fileReport = await ScanSingleFileAsync(file);
                    report.FileReports.Add(fileReport);

                    // Update vulnerability counts
                    report.TotalVulnerabilities += fileReport.Result.TotalVulnerabilities;

                    // Update risk level counts
                    if (fileReport.Result.Summary != null)
                    {
                        // Use dynamic to access the summary properties
                        dynamic summary = fileReport.Result.Summary;
                        report.VulnerabilitiesByRiskLevel["Critic"] += (int)summary.critic;
                        report.VulnerabilitiesByRiskLevel["Ridicat"] += (int)summary.ridicat;
                        report.VulnerabilitiesByRiskLevel["Mediu"] += (int)summary.mediu;
                        report.VulnerabilitiesByRiskLevel["Scazut"] += (int)summary.scazut;
                    }
                }
                catch (Exception ex)
                {
                    // Log error and continue with next file
                    Console.WriteLine($"Error analyzing file {file}: {ex.Message}");
                }
            }

            return report;
        }

        public async Task<FileVulnerabilityReport> ScanSingleFileAsync(string filePath)
        {
            // Read file content
            string code = await File.ReadAllTextAsync(filePath);

            // Get relative path for cleaner reporting
            string relativePath = Path.GetRelativePath(_projectRootPath, filePath);

            // Analyze code for vulnerabilities
            var analysisResult = await _vulnerabilityService.AnalyzeCodeAsync(code);

            return new FileVulnerabilityReport
            {
                FilePath = filePath,
                RelativePath = relativePath,
                Result = analysisResult
            };
        }
    }
}