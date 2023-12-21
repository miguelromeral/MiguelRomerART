using Microsoft.AspNetCore.Mvc;
using MRA.Services.Firebase.Interfaces;
using MRA.Web.Models;
using System.Diagnostics;
using MRA.Services.AzureStorage;
using MRA.Services;

namespace MRA.Web.Controllers
{
    public class ArtController : Controller
    {
        private readonly DrawingService _drawingService;
        private readonly ILogger<HomeController> _logger;

        public ArtController(ILogger<HomeController> logger, DrawingService drawingService)
        {
            _logger = logger;
            _drawingService = drawingService;
        }


        public async Task<IActionResult> Index()
        {
            var model = new IndexModel();

            model.Drawings = await _drawingService.GetAllDrawings();

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
