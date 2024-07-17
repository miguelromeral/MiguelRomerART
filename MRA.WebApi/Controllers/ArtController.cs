using Microsoft.AspNetCore.Mvc;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;
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

        [HttpGet("drawings")]
        public async Task<List<Drawing>> Details()
        {
            return await _drawingService.GetAllDrawings();
        }

        [HttpGet("select/products")]
        public async Task<List<ProductListItem>> Products()
        {
            var drawings = await _drawingService.GetAllDrawings();
            return _drawingService.GetProducts(drawings);
        }

        [HttpGet("select/characters")]
        public async Task<List<CharacterListItem>> Characters()
        {
            var drawings = await _drawingService.GetAllDrawings();
            return _drawingService.GetCharacters(drawings);
        }

        [HttpGet("select/models")]
        public async Task<List<string>> Models()
        {
            var drawings = await _drawingService.GetAllDrawings();
            return _drawingService.GetModels(drawings);
        }


        [HttpGet("collections")]
        public async Task<List<Collection>> Collections()
        {
            return await _drawingService.GetAllCollections();
        }


        [HttpGet("details/{id}")]
        public async Task<Drawing> Details(string id)
        {
            //var model = new DetailsModel(id, SessionSettings.IsLogedAsAdmin(HttpContext.Session.GetString(SessionSettings.USER_ID)));
            return await _drawingService.FindDrawingById(id, updateViews: true, cache: false);
        }


        [HttpPost("filter")]
        public async Task<List<Drawing>> Filter([FromBody] DrawingFilter filters)
        {
            return await _drawingService.FilterDrawings(filters);
        }


        [HttpPost("cheer")]
        public async Task Cheer([FromBody] string id)
        {
            await _drawingService.UpdateLikes(id);
        }

        [HttpPost("vote/{id}")]
        public async Task<VoteSubmittedModel> Vote(string id, [FromBody] int score)
        {
            return await _drawingService.Vote(id, score);
        }
    }
}
