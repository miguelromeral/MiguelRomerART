using Microsoft.AspNetCore.Mvc;
using MRA.Services;
using MRA.Services.Firebase;
using MRA.Services.Firebase.Interfaces;
using MRA.Web.Models;
using System.Diagnostics;

namespace MRA.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly AzureStorageService _storageService;
        private readonly IFirestoreService _firestoreService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AzureStorageService storageService, IFirestoreService firestoreService)
        {
            _logger = logger;
            _storageService = storageService;
            _firestoreService = firestoreService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexModel(_firestoreService);
            await model.OnGetAsync();
            model.Blobs = await _storageService.ListBlobFilesAsync();

            return View(model);
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
