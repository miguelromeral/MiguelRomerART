using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Services;
using MRA.Services.Firebase.Models;
using MRA.WebApi.Models.Responses;

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
            // TODO: VOLVER A PONER EL UPDATE VIEWS A TRUE Y EL CACHE A FALSE
            return await _drawingService.FindDrawingById(id, updateViews: false, cache: true);
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




        [Authorize]
        [HttpPost("save/{id}")]
        public async Task<SaveDrawingRequest> SaveDrawing(string id, [FromBody] SaveDrawingRequest request)
        {
            try
            {
                //Thread.Sleep(1000);
                return request;

                //var drawing = model.Drawing;
                //var userId = HttpContext.Session.GetString(SessionSettings.USER_ID) ?? "";
                //if (!SessionSettings.IsLogedAsAdmin(userId))
                //{
                //    ViewBag.Error = "No eres administrador y no tienes permiso para editarlo";
                //    return null;
                //}

                //drawing.Name = drawing.Name ?? "";
                //drawing.ModelName = drawing.ModelName ?? "";
                //drawing.ProductName = drawing.ProductName ?? "";
                //drawing.ReferenceUrl = drawing.ReferenceUrl ?? "";
                //drawing.Path = drawing.Path ?? "";
                //drawing.PathThumbnail = _drawingService.CrearThumbnailName(drawing.Path);
                //drawing.Title = drawing.Title ?? "";
                //drawing.SpotifyUrl = drawing.SpotifyUrl ?? "";
                //drawing.Tags = (drawing.TagsText ?? "").Split(Drawing.SEPARATOR_TAGS).ToList();

                //Drawing result = null;
                //if (!String.IsNullOrEmpty(drawing.Id))
                //{
                //    result = await _drawingService.AddAsync(drawing);
                //    return drawing;
                //}

                //if (result == null)
                //{
                //    ViewBag.Error = "Ha ocurrido un error por el que no se ha guardado el dibujo";
                //    return null;
                //}

                //_drawingService.CleanAllCache();
                //return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpGet("checkdrawing/{id}")]
        public async Task<bool> ExisteDrawingId(string id)
        {
            var drawing = await _drawingService.FindDrawingById(id);
            return drawing != null;
        }


        [Authorize]
        [HttpPost("checkazurepath")]
        public async Task<JsonResult> CheckAzurePath([FromBody] CheckAzurePathRequest request)
        {
            var existe = await _drawingService.ExistsBlob(request.Id);

            var blobLocationThumbnail = _drawingService.CrearThumbnailName(request.Id);

            var urlBase = _drawingService.GetAzureUrlBase();
            var url = urlBase + request.Id;
            var url_tn = urlBase + blobLocationThumbnail;

            return new JsonResult(new
            {
                existe,
                url,
                url_tn
            });
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<UploadAzureImageResponse> UploadAzureImage([FromForm] IFormFile image, [FromForm] int size, [FromForm] string path)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return new UploadAzureImageResponse()
                    {
                        Ok = false,
                        Error = "No se ha proporcionado ninguna imagen",
                    };
                }

                var blobLocationThumbnail = _drawingService.CrearThumbnailName(path);
                await UploadImage(image, blobLocationThumbnail, size);
                await UploadImage(image, path, 0);

                var urlBase = _drawingService.GetAzureUrlBase();
                var url = urlBase + path;
                var url_tn = urlBase + blobLocationThumbnail;


                return new UploadAzureImageResponse()
                {
                    Ok = true,
                    Error = "",
                    Url = url,
                    UrlThumbnail = url_tn,
                    PathThumbnail = blobLocationThumbnail
                };
            }
            catch (Exception ex)
            {
                return new UploadAzureImageResponse()
                {
                    Ok = false,
                    Error = ex.Message,
                };
            }
        }

        private async Task UploadImage(IFormFile azureImage, string path, int azureImageThumbnailSize)
        {
            using (var imageStream = new MemoryStream())
            {
                await azureImage.CopyToAsync(imageStream);
                imageStream.Position = 0;

                await _drawingService.RedimensionarYGuardarEnAzureStorage(imageStream, path, azureImageThumbnailSize);
            }
        }

    }
}
