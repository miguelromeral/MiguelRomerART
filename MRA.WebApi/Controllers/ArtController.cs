using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Services;
using MRA.Services.Firebase.Models;
using MRA.WebApi.Models.Requests;
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
        public async Task<List<CollectionResponse>> Collections()
        {
            return (await _drawingService.GetAllCollections()).Select(x => new CollectionResponse(x)).ToList();
        }


        [HttpGet("collection/details/{id}")]
        public async Task<CollectionResponse> CollectionDetails(string id)
        {
            return new CollectionResponse(await _drawingService.FindCollectionById(id));
        }


        [HttpGet("details/{id}")]
        public async Task<Drawing> Details(string id)
        {
            return await _drawingService.FindDrawingById(id, true, updateViews: true, cache: false); ;
        }


        [HttpGet("details-admin/{id}")]
        [Authorize]
        public async Task<Drawing> DetailsAdmin(string id)
        {
            return await _drawingService.FindDrawingById(id, false, updateViews: false, cache: false);
        }


        [HttpPost("filter-public")]
        public async Task<List<Drawing>> Filter([FromBody] DrawingFilter filters)
        {
            filters.OnlyVisible = true;
            return await _drawingService.FilterDrawings(filters);
        }

        [HttpPost("filter-admin")]
        [Authorize]
        public async Task<List<Drawing>> FilterAdmin([FromBody] DrawingFilter filters)
        {
            filters.OnlyVisible = false;
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
        public async Task<Drawing> SaveDrawing(string id, [FromBody] SaveDrawingRequest request)
        {
            try
            {
                var drawing = new Drawing()
                {
                    Comment = string.Join(Drawing.SEPARATOR_COMMENTS, request.ListComments),
                    CommentCons = string.Join(Drawing.SEPARATOR_COMMENTS, request.ListCommentCons),
                    CommentPros = string.Join(Drawing.SEPARATOR_COMMENTS, request.ListCommentPros),
                    Date = request.DateHyphen.Replace("-", "/"),
                    DateHyphen = request.DateHyphen,
                    Favorite = request.Favorite,
                    Id = request.Id,
                    ModelName = request.ModelName,
                    Name = request.Name,
                    Paper = request.Paper,
                    Path = request.Path,
                    PathThumbnail = request.PathThumbnail,
                    ProductName = request.ProductName,
                    ProductType = request.ProductType,
                    ReferenceUrl = request.ReferenceUrl,
                    ScoreCritic = request.ScoreCritic,
                    Software = request.Software,
                    SpotifyUrl = request.SpotifyUrl,
                    Tags = request.TagsText.Split(Drawing.SEPARATOR_TAGS).ToList(),
                    Time = request.Time,
                    Title = request.Title,
                    Type = request.Type,
                    InstagramUrl = request.InstagramUrl,
                    TwitterUrl = request.TwitterUrl,
                    Visible = request.Visible
                };
               
                Drawing result = await _drawingService.AddAsync(drawing);
                result.TagsText = string.Join(Drawing.SEPARATOR_TAGS, result.Tags);
                _drawingService.CleanAllCache();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpGet("checkdrawing/{id}")]
        public async Task<bool> ExisteDrawingId(string id)
        {
            var drawing = await _drawingService.FindDrawingById(id, false);
            return drawing != null;
        }

        [Authorize]
        [HttpPost("checkazurepath")]
        public async Task<CheckAzurePathResponse> CheckAzurePath([FromBody] CheckAzurePathRequest request)
        {
            var existe = await _drawingService.ExistsBlob(request.Id);

            var blobLocationThumbnail = _drawingService.CrearThumbnailName(request.Id);

            var urlBase = _drawingService.GetAzureUrlBase();
            var url = urlBase + request.Id;
            var url_tn = urlBase + blobLocationThumbnail;

            return new CheckAzurePathResponse()
            {
                Existe = existe,
                Url = url,
                UrlThumbnail = url_tn,
                PathThumbnail = blobLocationThumbnail
            };
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






        [HttpGet("check/collection/{id}")]
        public async Task<bool> ExisteCollectionId(string id)
        {
            try
            {
                var collection = await _drawingService.FindCollectionById(id);
                return collection != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost("save/collection/{id}")]
        [Authorize]
        public async Task<CollectionResponse> SaveCollection(string id, [FromBody] SaveCollectionRequest model)
        {
            try
            {
                var collection = new Collection()
                {
                    Id = model.Id,
                    Description = model.Description,
                    Name = model.Name,
                    Order = model.Order
                };
                collection.DrawingsReferences = await _drawingService.SetDrawingsReferences(model.DrawingsIds);

                if (!String.IsNullOrEmpty(collection.Id))
                {
                    Collection result = await _drawingService.AddAsync(collection);
                    _drawingService.CleanAllCache();
                    return new CollectionResponse(result);
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpPost("collection/remove")]
        [Authorize]
        public async Task<bool> RemoveCollection([FromBody] string id)
        {
            return await _drawingService.RemoveCollection(id);
        }
    }
}
