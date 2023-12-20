using Microsoft.AspNetCore.Mvc;
using MRA.Services;
using MRA.Web.Models;
using System.Diagnostics;

namespace MRA.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly AzureStorageService _storageService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AzureStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService;
        }

        public async Task<IActionResult> Index()
        {
            var blobFiles = await _storageService.ListBlobFilesAsync();

            // Puedes realizar más lógica si es necesario

            return View(blobFiles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
