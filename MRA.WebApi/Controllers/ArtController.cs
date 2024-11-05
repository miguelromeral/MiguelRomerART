using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Services;
using MRA.DTO.Firebase.Models;
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

        [HttpGet("select/products")]
        public async Task<List<ProductListItem>> Products()
        {
            try
            {
                _logger.LogInformation("Solicitando lista de productos");
                var drawings = await _drawingService.GetAllDrawings();
                var products = _drawingService.GetProducts(drawings);
                _logger.LogInformation("Productos recuperados: " + products.Count);
                return products;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar la lista de productos");
                return new List<ProductListItem>();
            }
        }

        [HttpGet("select/characters")]
        public async Task<List<CharacterListItem>> Characters()
        {
            try
            {
                _logger.LogInformation("Solicitando lista de personajes");
                var drawings = await _drawingService.GetAllDrawings();
                var characters = _drawingService.GetCharacters(drawings);
                _logger.LogInformation($"Personajes recuperados: {characters.Count}");
                return characters;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar la lista de personajes");
                return new List<CharacterListItem>();
            }
        }

        [HttpGet("select/models")]
        public async Task<List<string>> Models()
        {
            try
            {
                _logger.LogInformation("Solicitando lista de modelos");
                var drawings = await _drawingService.GetAllDrawings();
                var models = _drawingService.GetModels(drawings);
                _logger.LogInformation($"Modelos recuperados: {models.Count}");
                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar la lista de modelos");
                return new List<string>();
            }
        }


        [HttpGet("collections-public")]
        public async Task<List<CollectionResponse>> Collections()
        {
            try
            {
                _logger.LogInformation("Solicitando lista de colecciones públicas");
                var drawings = await _drawingService.GetAllDrawings();
                var collections = (await _drawingService.GetAllCollections(drawings)).Select(x => new CollectionResponse(x)).ToList();
                _logger.LogDebug($"Colecciones leídas: {collections.Count}. Ahora se filtrarán solo las públicas");
                var publicCollections = new List<CollectionResponse>();
                foreach (var collection in collections)
                {
                    publicCollections.Add(FilterCollectionResponsePublic(collection));
                }
                _logger.LogInformation($"Colecciones públicas recuperadas: {publicCollections.Count}");
                return publicCollections;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar la lista de colecciones públicas");
                return new List<CollectionResponse>();
            }
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
            try
            {
                _logger.LogInformation("Solicitando lista completa de colecciones");
                var drawings = await _drawingService.GetAllDrawings();
                var collections = (await _drawingService.GetAllCollections(drawings)).Select(x => new CollectionResponse(x)).ToList();
                _logger.LogInformation($"Colecciones recuperadas: {collections.Count}");
                return collections;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar la lista completa de colecciones");
                return new List<CollectionResponse>();
            }
        }


        [HttpGet("collection/details-public/{id}")]
        public async Task<CollectionResponse> CollectionDetails(string id)
        {
            try
            {
                _logger.LogInformation($"Solicitando detalles públicos de la colección \"{id}\"");
                var drawings = await _drawingService.GetAllDrawings();
                var collection = await _drawingService.FindCollectionById(id, drawings);
                _logger.LogDebug($"Colección \"{id}\" recuperada. Ahora filtrando si es pública");
                var publicCollection = FilterCollectionResponsePublic(new CollectionResponse(collection));
                if (String.IsNullOrEmpty(publicCollection.Id))
                {
                    _logger.LogWarning($"Colección \"{id}\" parece ser privada. NO se entrega información a FRONT");
                }
                else
                {
                    _logger.LogDebug($"Colección \"{id}\" es pública, se entrega información a FRONT");
                }
                return publicCollection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al recuperar detalles públicos de colección \"{id}\"");
                return new CollectionResponse(new Collection());
            }
        }

        [HttpGet("collection/details-admin/{id}")]
        [Authorize]
        public async Task<CollectionResponse> CollectionDetailsAdmin(string id)
        {
            try
            {
                _logger.LogInformation($"Solicitando detalles privados de la colección \"{id}\"");
                var drawings = await _drawingService.GetAllDrawings();
                var collection = await _drawingService.FindCollectionById(id, drawings);
                _logger.LogInformation($"Encontrados detalles de colección \"{id}\"");
                return new CollectionResponse(collection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al recuperar detalles privados de colección \"{id}\"");
                return new CollectionResponse(new Collection());
            }
        }


        [HttpGet("details/{id}")]
        public async Task<Drawing> Details(string id)
        {
            try
            {
                _logger.LogInformation($"Solicitando detalles públicos de dibujo \"{id}\"");
                Drawing? drawing = await _drawingService.FindDrawingById(id, onlyIfVisible: true, updateViews: true, cache: false);
                if(drawing == null)
                {
                    _logger.LogError($"No se encontraron detalles públicos del dibujo \"{id}\". ¿Puede ser privado?");
                    return new Drawing();
                }
                else
                {
                    _logger.LogInformation($"Recuperados detalles públicos del dibujo \"{drawing}\"");
                }
                return drawing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al recuperar detalles públicos del dibujo \"{id}\"");
                return new Drawing();
            }
        }


        [HttpGet("details-admin/{id}")]
        [Authorize]
        public async Task<Drawing> DetailsAdmin(string id)
        {
            try
            {
                _logger.LogInformation($"Solicitando detalles privados de dibujo \"{id}\"");
                Drawing drawing = await _drawingService.FindDrawingById(id, onlyIfVisible: false, updateViews: false, cache: false);
                if (drawing == null)
                {
                    _logger.LogError($"No se encontraron detalles privados del dibujo \"{id}\"");
                    return new Drawing();
                }
                else
                {
                    _logger.LogInformation($"Recuperados detalles privados del dibujo \"{drawing}\"");
                }
                return drawing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al recuperar detalles privados del dibujo \"{id}\"");
                return new Drawing();
            }
        }


        [HttpPost("filter-public")]
        public async Task<DrawingFilterResultsResponse> Filter([FromBody] DrawingFilter filters)
        {
            return await FilterDrawings(filters, true);
        }

        [HttpPost("filter-admin")]
        [Authorize]
        public async Task<DrawingFilterResultsResponse> FilterAdmin([FromBody] DrawingFilter filters)
        {
            return await FilterDrawings(filters, false);
        }

        private async Task<DrawingFilterResultsResponse> FilterDrawings(DrawingFilter filters, bool onlyVisible)
        {
            try
            {
                _logger.LogInformation($"Filtrando dibujos. ¿Sólo públicos? \"{(onlyVisible ? "SÍ" : "NO")}\"");
                _logger.LogDebug($"Filtros utilizados: {filters.CacheKey}");
                var allDrawings = await _drawingService.GetAllDrawings();
                _logger.LogDebug($"Total Dibujos: {allDrawings.Count}");
                var allCollections = await _drawingService.GetAllCollections(allDrawings);
                _logger.LogDebug($"Total Colecciones: {allCollections.Count}");
                filters.OnlyVisible = onlyVisible;
                _logger.LogDebug("Ahora se filtran dibujos y colecciones según criterios");
                var filteredDrawings =
                    new DrawingFilterResultsResponse(await _drawingService.FilterDrawingsGivenList(filters, allDrawings, allCollections));
                _logger.LogInformation($"Dibujos filtrados: {filteredDrawings.FetchedCount}/{filteredDrawings.TotalCount} [{filters.PageNumber}]({filters.PageSize})");
                return filteredDrawings;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar los dibujos");
                return new DrawingFilterResultsResponse(new FilterResults());
            }
        }

        [HttpPost("cheer")]
        public async Task Cheer([FromBody] string id)
        {
            try
            {
                _logger.LogInformation($"Dibujo \"{id}\" recibió un like! ❤");
                await _drawingService.UpdateLikes(id);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar like al dibujo \"{id}\"");
            }
        }

        [HttpPost("vote/{id}")]
        public async Task<VoteSubmittedModel> Vote(string id, [FromBody] int score)
        {
            try
            {
                _logger.LogInformation($"Dibujo \"{id}\" recibió un voto! 🗳 [{score}]");
                var newScore = await _drawingService.Vote(id, score);
                _logger.LogInformation($"Dibujo \"{id}\" ahora tiene {newScore.NewScore} ({newScore.NewVotes} votos)");
                return newScore;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar voto al dibujo \"{id}\"");
                return new VoteSubmittedModel();
            }
        }

        [Authorize]
        [HttpPost("save/{id}")]
        public async Task<Drawing> SaveDrawing(string id, [FromBody] SaveDrawingRequest request)
        {
            try
            {
                _logger.LogInformation($"Guardando dibujo \"{request.Id}\"");
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
                _logger.LogDebug("Limpiando la caché para que se vean los cambios");
                _drawingService.CleanAllCache();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al guardar el dibujo \"{request.Id}\"");
                return null;
            }
        }


        [HttpGet("checkdrawing/{id}")]
        public async Task<bool> ExisteDrawingId(string id)
        {
            try
            {
                _logger.LogInformation($"Comprobando si existe dibujo \"{id}\"");
                var drawing = await _drawingService.FindDrawingById(id, onlyIfVisible: false);
                var existe = drawing != null;
                _logger.LogInformation($"¿Existe dibujo \"{id}\"? {(existe ? "SÍ" : "NO")}");
                return existe;
            }catch(Exception ex)
            {
                _logger.LogError(ex, $"Error al comprobar si existe dibujo \"{id}\". Se indica sí para evitar errores");
                return true;
            }
        }

        [Authorize]
        [HttpPost("checkazurepath")]
        public async Task<CheckAzurePathResponse> CheckAzurePath([FromBody] CheckAzurePathRequest request)
        {
            try
            {
                _logger.LogInformation($"Comprobando si la ruta de Azure es válida: \"{request.Id}\"");
                var existe = await _drawingService.ExistsBlob(request.Id);
                _logger.LogDebug($"¿Existe ruta en Azure \"{request.Id}\"? {(existe ? "SÍ" : "NO")}");

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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al comprobar si la ruta de Azure \"{request.Id}\"");
                return new CheckAzurePathResponse();
            }
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<UploadAzureImageResponse> UploadAzureImage([FromForm] IFormFile image, [FromForm] int size, [FromForm] string path)
        {
            try
            {
                _logger.LogInformation($"Solicitada subida de imagen \"{image.FileName}\" a Azure: \"{path}\"");
                if (image == null || image.Length == 0)
                {
                    _logger.LogError("No se ha proporcionado ninguna imagen");
                    return new UploadAzureImageResponse()
                    {
                        Ok = false,
                        Error = "No se ha proporcionado ninguna imagen",
                    };
                }

                var blobLocationThumbnail = _drawingService.CrearThumbnailName(path);
                _logger.LogDebug($"Subiendo Thumbnail a Azure: {blobLocationThumbnail}");
                await UploadImage(image, blobLocationThumbnail, size);
                _logger.LogDebug($"Subiendo Imagen a Azure: {blobLocationThumbnail}");
                await UploadImage(image, path, 0);

                var urlBase = _drawingService.GetAzureUrlBase();
                var url = urlBase + path;
                var url_tn = urlBase + blobLocationThumbnail;

                _logger.LogInformation($"Imagen \"{image.FileName}\" subida a Azure con éxito");
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
                _logger.LogError(ex, $"Error al subir la imagen \"{image.FileName}\" a Azure");
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
                _logger.LogInformation($"Comprobando si existe colección \"{id}\"");
                var drawings = await _drawingService.GetAllDrawings();
                var collection = await _drawingService.FindCollectionById(id, drawings);
                var existe = collection != null;
                _logger.LogInformation($"¿Existe colección \"{id}\"? {(existe ? "SÍ" : "NO")}");
                return existe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al comprobar si la colección \"{id}\" existe");
                return false;
            }
        }

        [HttpPost("save/collection/{id}")]
        [Authorize]
        public async Task<CollectionResponse> SaveCollection(string id, [FromBody] SaveCollectionRequest model)
        {
            try
            {
                _logger.LogInformation($"Solicitado guardado de colección \"{model.Id}\"");
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

                _logger.LogError("No existe ID para la colección a guardar");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al guardar la colección \"{model.Id}\"");
            }
            return null;
        }


        [HttpPost("collection/remove")]
        [Authorize]
        public async Task<bool> RemoveCollection([FromBody] string id)
        {
            try
            {
                _logger.LogInformation($"Solicitada eliminación de colección \"{id}\"");
                await _drawingService.RemoveCollection(id);
                _logger.LogInformation($"Colección \"{id}\" eliminada con éxito");
                _drawingService.CleanAllCache();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la colección \"{id}\"");
            }
            return false;
        }
    }
}
