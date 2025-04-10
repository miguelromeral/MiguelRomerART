using Microsoft.AspNetCore.Mvc;
using MRA.Services;
using MRA.Services.AzureStorage;
using MRA.Services.Firebase;
using MRA.Services.Firebase.Interfaces;
using MRA.Web.Models;
using MRA.Web.Models.Home;
using System.Diagnostics;

namespace MRA.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppService _drawingService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AppService drawingService)
        {
            _logger = logger;
            _drawingService = drawingService;
        }

        public async Task<IActionResult> Index()
        {
            var drawings = await _drawingService.GetAllDrawings();
            var model = new IndexViewModel()
            {
                Collections = await _drawingService.GetAllCollections(drawings)
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Settings()
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
