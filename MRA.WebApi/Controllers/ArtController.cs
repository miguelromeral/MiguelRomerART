using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Services;
using MRA.DTO.Firebase.Models;
using MRA.WebApi.Models.Requests;
using MRA.WebApi.Models.Responses;
using MRA.DTO.Logger;

namespace MRA.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtController : Controller
    {

        private readonly DrawingService _drawingService;
        private readonly MRLogger _logger;

        public ArtController(MRLogger logger, DrawingService drawingService)
        {
            _logger = logger;
            _drawingService = drawingService;
        }

        [HttpGet("select/products")]
        public async Task<List<ProductListItem>> Products()
        {
            try
            {
                _logger.Info("Solicitados Productos");
                var drawings = await _drawingService.GetAllDrawings();
                var products = _drawingService.GetProducts(drawings);
                _logger.Success("Productos recuperados: " + products.Count);
                return products;
            }
            catch(Exception ex)
            {
                _logger.Error("Error al recuperar los productos: "+ex.Message);
                return new List<ProductListItem>();
            }
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


        [HttpGet("collections-public")]
        public async Task<List<CollectionResponse>> Collections()
        {
            var drawings = await _drawingService.GetAllDrawings();
            var collections = (await _drawingService.GetAllCollections(drawings)).Select(x => new CollectionResponse(x)).ToList();
            var newList = new List<CollectionResponse>();
            foreach(var collection in collections)
            {
                newList.Add(FilterCollectionResponsePublic(collection));
            }
            return newList;
        }

        private CollectionResponse FilterCollectionResponsePublic(CollectionResponse collection)
        {
            collection.Drawings = collection.Drawings.Where(x => x.Visible).ToList();
            collection.DrawingsId = collection.Drawings.Select(x => x.Id).ToList();
            return collection;
        }


        [HttpGet("collections-admin")]
        [Authorize]
        public async Task<List<CollectionResponse>> CollectionsAdmin()
        {
            var drawings = await _drawingService.GetAllDrawings();
            return (await _drawingService.GetAllCollections(drawings)).Select(x => new CollectionResponse(x)).ToList();
        }


        [HttpGet("collection/details-public/{id}")]
        public async Task<CollectionResponse> CollectionDetails(string id)
        {
            var drawings = await _drawingService.GetAllDrawings();
            return FilterCollectionResponsePublic(new CollectionResponse(await _drawingService.FindCollectionById(id, drawings)));
        }

        [HttpGet("collection/details-admin/{id}")]
        [Authorize]
        public async Task<CollectionResponse> CollectionDetailsAdmin(string id)
        {
            var drawings = await _drawingService.GetAllDrawings();
            return new CollectionResponse(await _drawingService.FindCollectionById(id, drawings));
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
        public async Task<DrawingFilterResultsResponse> Filter([FromBody] DrawingFilter filters)
        {
            var allDrawings = await _drawingService.GetAllDrawings();
            var allCollections = await _drawingService.GetAllCollections(allDrawings);
            filters.OnlyVisible = true;
            return new DrawingFilterResultsResponse(await _drawingService.FilterDrawingsGivenList(filters, allDrawings, allCollections));
        }

        [HttpPost("filter-admin")]
        [Authorize]
        public async Task<DrawingFilterResultsResponse> FilterAdmin([FromBody] DrawingFilter filters)
        {
            var allDrawings = await _drawingService.GetAllDrawings();
            var allCollections = await _drawingService.GetAllCollections(allDrawings);
            filters.OnlyVisible = false;
            return new DrawingFilterResultsResponse(await _drawingService.FilterDrawingsGivenList(filters, allDrawings, allCollections));
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
                    ListComments = request.ListComments,
                    ListCommentsStyle = request.ListCommentsStyle,
                    ListCommentsPros = request.ListCommentsPros,
                    ListCommentsCons = request.ListCommentsCons,
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
                    Filter = request.Filter,
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
                var drawings = await _drawingService.GetAllDrawings();
                var collection = await _drawingService.FindCollectionById(id, drawings);
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
                    var drawings = await _drawingService.GetAllDrawings();
                    Collection result = await _drawingService.AddAsync(collection, drawings);
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
            var result = await _drawingService.RemoveCollection(id);
            if (result)
            {
                _drawingService.CleanAllCache();
            }
            return result;
        }
    }
}
