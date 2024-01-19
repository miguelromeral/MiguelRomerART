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


        public async Task<IActionResult> Index(string TextQuery, string Type, string ProductType, string ProductName, string Collection,
            string CharacterName, string ModelName, string Software, string Paper, string Sortby, bool Favorites)
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
                Query_Favorites = Favorites,
                Query_Sortby = Sortby
            };

            model.Drawings = await _drawingService.GetAllDrawings();

            model.ProductNameSelect = new Dictionary<string, int>();

            foreach (var product in model.Drawings.Where(x => !String.IsNullOrEmpty(x.ProductName)).Select(x => new { x.ProductName, x.ProductType }).Distinct().ToList())
            {
                if (!model.ProductNameSelect.ContainsKey(product.ProductName))
                {
                    model.ProductNameSelect.Add(product.ProductName, product.ProductType);
                }
            }


            model.CharacterNameSelect = new Dictionary<string, int>();

            foreach (var product in model.Drawings.Where(x => !String.IsNullOrEmpty(x.Name)).Select(x => new { x.Name, x.ProductType }).Distinct().ToList())
            {
                if (!model.CharacterNameSelect.ContainsKey(product.Name))
                {
                    model.CharacterNameSelect.Add(product.Name, product.ProductType);
                }
            }


            model.ModelNameSelect = new List<string>();

            foreach (var modelName in model.Drawings.Where(x => !String.IsNullOrEmpty(x.ModelName)).Select(x => x.ModelName).Distinct().ToList())
            {
                if (!model.ModelNameSelect.Contains(modelName))
                {
                    model.ModelNameSelect.Add(modelName);
                }
            }

            model.ListCollections = await _drawingService.GetAllCollections();

            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var model = new DetailsModel(id);
            model.Drawing = await _drawingService.FindDrawingById(id, false);
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
