using Microsoft.AspNetCore.Mvc;
using MRA.Services;
using MRA.Web.Models.About;
using MRA.Web.Models.Home;

namespace MRA.Web.Controllers
{
    public class AboutController : Controller
    {


        private readonly DrawingService _drawingService;
        private readonly ILogger<HomeController> _logger;

        public AboutController(ILogger<HomeController> logger, DrawingService drawingService)
        {
            _logger = logger;
            _drawingService = drawingService;
        }


        public IActionResult Index()
        {
            return RedirectToAction("Me");
        }

        public async Task<IActionResult> Me()
        {
            return View(new AboutViewModel() { Inspirations = await _drawingService.GetAllInspirations() });
        }
    }
}
