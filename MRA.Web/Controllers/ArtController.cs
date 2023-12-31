using Microsoft.AspNetCore.Mvc;
using MRA.Services.Firebase.Interfaces;
using MRA.Web.Models;
using System.Diagnostics;
using MRA.Services.AzureStorage;
using MRA.Services;
using MRA.Web.Models.Art;
using MRA.Services.Firebase.Models;

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

            model.ProductNames = model.Drawings.Select(x => x.ProductName).Distinct().ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var model = new DetailsModel(id);
            model.Drawing = await _drawingService.FindDrawingById(id);
            return View(model);
        }

        [HttpPost]
        public async Task<List<Drawing>> Filter(FilterDrawingModel filters)
        {
            return await _drawingService.FilterDrawings(filters);
        }

        [HttpPost]
        public async Task Cheer(string id)
        {
            await _drawingService.UpdateLikes(id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
