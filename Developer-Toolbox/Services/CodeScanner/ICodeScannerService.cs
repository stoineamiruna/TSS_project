using Developer_Toolbox.Services;
using System.Threading.Tasks;

namespace Developer_Toolbox.Services.CodeScanner
{
    public interface ICodeScannerService
    {
        Task<ProjectVulnerabilityReport> ScanProjectAsync(string projectDirectory = null);
        Task<FileVulnerabilityReport> ScanSingleFileAsync(string filePath);
    }
}