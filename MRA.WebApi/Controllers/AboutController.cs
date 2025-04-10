using Microsoft.AspNetCore.Mvc;
using MRA.DTO.Models;
using MRA.Services.Models.Inspirations;

namespace MRA.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AboutController : Controller
    {
        private readonly ILogger<AboutController> _logger;
        private readonly IInspirationService _inspirationService;

        public AboutController(
            ILogger<AboutController> logger, 
            IInspirationService inspirationService)
        {
            _logger = logger;
            _inspirationService = inspirationService;
        }


        [HttpGet("inspirations")]
        public async Task<IEnumerable<InspirationModel>> IndexAsync()
        {
            return await _inspirationService.GetAllInspirationsAsync();
        }
    }
}
