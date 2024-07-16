using Microsoft.AspNetCore.Mvc;
using MRA.Services;
using MRA.Services.Firebase.Models;

namespace MRA.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtController : Controller
    {

        private readonly DrawingService _drawingService;
        private readonly ILogger<ArtController> _logger;

        public ArtController(ILogger<ArtController> logger, DrawingService drawingService)
        {
            _logger = logger;
            _drawingService = drawingService;
        }

        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet("details/{id}")]
        public async Task<Drawing> Details(string id)
        {
            //var model = new DetailsModel(id, SessionSettings.IsLogedAsAdmin(HttpContext.Session.GetString(SessionSettings.USER_ID)));
            return await _drawingService.FindDrawingById(id, updateViews: true, cache: false);
        }
    }
}
