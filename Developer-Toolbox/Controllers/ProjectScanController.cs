using System;
using System.Threading.Tasks;
using Developer_Toolbox.Services;
using Developer_Toolbox.Services.CodeScanner;
using Microsoft.AspNetCore.Mvc;

namespace Developer_Toolbox.Controllers
{
    public class ProjectScanController : Controller
    {
        private readonly ICodeScannerService _codeScannerService;

        public ProjectScanController(ICodeScannerService codeScannerService)
        {
            _codeScannerService = codeScannerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ScanProject()
        {
            try
            {
                var report = await _codeScannerService.ScanProjectAsync();
                return View("ScanResults", report);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error scanning project: {ex.Message}";
                return View("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ScanFile(string filePath)
        {
            try
            {
                var fileReport = await _codeScannerService.ScanSingleFileAsync(filePath);
                return View("FileScanResult", fileReport);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error scanning file: {ex.Message}";
                return View("Index");
            }
        }
    }
}