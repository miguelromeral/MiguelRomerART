using Microsoft.AspNetCore.Mvc;
using MRA.Services;
using MRA.Services.Firebase.Models;

namespace MRA.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AboutController : Controller
    {

        private readonly DrawingService _drawingService;
        private readonly ILogger<AboutController> _logger;

        public AboutController(ILogger<AboutController> logger, DrawingService drawingService)
        {
            _logger = logger;
            _drawingService = drawingService;
        }


        [HttpGet("inspirations")]
        public async Task<List<Inspiration>> IndexAsync()
        {
            return await _drawingService.GetAllInspirations();
        }
    }
}
