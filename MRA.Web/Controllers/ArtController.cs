using Microsoft.AspNetCore.Mvc;
using MRA.DTO.Firebase.Interfaces;
using MRA.Web.Models;
using System.Diagnostics;
using MRA.DTO.AzureStorage;
using MRA.Services;
using MRA.Web.Models.Art;
using MRA.DTO.Firebase.Models;
using Azure.Storage.Blobs.Models;
using Google.Api;
using MRA.Web.Utils;
using MRA.DTO.ViewModels.Art;

namespace MRA.Web.Controllers
{
    public class ArtController : Controller
    {
        private readonly DrawingService _drawingService;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public ArtController(ILogger<HomeController> logger, DrawingService drawingService, IConfiguration configuration)
        {
            _logger = logger;
            _drawingService = drawingService;
            _configuration = configuration;
        }


        public async Task<IActionResult> Index(string TextQuery, string Type, string ProductType, string ProductName, string Collection,
            string CharacterName, string ModelName, string Software, string Paper, string Spotify, string Sortby, bool Favorites)
        {
            var model = new IndexModel()
            {
                Query_TextQuery = TextQuery,
                Query_Type = Type,
                Query_ProductType = ProductType,
                Query_ProductName = ProductName,
                Query_Collection = Collection,
                Query_CharacterName = CharacterName,
                Query_ModelName = ModelName,
                Query_Software = Software,
                Query_Paper = Paper,
                Query_Spotify = Spotify,
                Query_Favorites = Favorites,
                Query_Sortby = Sortby
            };

            model.Drawings = await _drawingService.GetAllDrawings();

            model.ProductNameSelect = _drawingService.GetProducts(model.Drawings).ToDictionary(x => x.ProductName, x => x.ProductTypeId);


            model.CharacterNameSelect = _drawingService.GetCharacters(model.Drawings).ToDictionary(x => x.CharacterName, x => x.ProductTypeId);


            model.ModelNameSelect = _drawingService.GetModels(model.Drawings);

            model.ListCollections = await _drawingService.GetAllCollections(model.Drawings);

            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var model = new DetailsModel(id, SessionSettings.IsLogedAsAdmin(HttpContext.Session.GetString(SessionSettings.USER_ID)));
            model.Drawing = await _drawingService.FindDrawingById(id, true, updateViews: true, cache: false);
            return View(model);
        }

        [HttpPost]
        public async Task<List<Drawing>> Filter(DrawingFilter filters)
        {
            return await _drawingService.FilterDrawings(filters);
        }

        [HttpPost]
        public async Task Cheer(string id)
        {
            await _drawingService.UpdateLikes(id);
        }

        [HttpPost]
        public async Task<VoteSubmittedModel> Vote(string id, int score)
        {
            return await _drawingService.Vote(id, score);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
