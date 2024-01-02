using Microsoft.AspNetCore.Mvc;
using MRA.Services;
using MRA.Services.AzureStorage;
using MRA.Services.Firebase;
using MRA.Services.Firebase.Interfaces;
using MRA.Web.Models;
using System.Diagnostics;

namespace MRA.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DrawingService _drawingService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DrawingService drawingService)
        {
            _logger = logger;
            _drawingService = drawingService;
        }

        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Index", "Art");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            return View(new AboutViewModel() { Inspirations = await _drawingService.GetAllInspirations() });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
