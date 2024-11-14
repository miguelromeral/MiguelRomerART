using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Services;
using MRA.DTO.Firebase.Models;
using MRA.WebApi.Models.Requests;
using MRA.WebApi.Models.Responses;
using MRA.DTO.Logger;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace MRA.WebApi.Controllers
{
    [ApiController]
    [Route("api/art")]
    public class ArtController : Controller
    {

        private readonly IDrawingService _drawingService;
        private readonly ILogger<ArtController> _logger;

        public ArtController(ILogger<ArtController> logger, IDrawingService drawingService)
        {
            _logger = logger;
            _drawingService = drawingService;
        }

        #region Drawing Selects
        [HttpGet("drawing/products")]
        public async Task<ActionResult<List<ProductListItem>>> DrawingProducts()
        {
            try
            {
                _logger.LogInformation("Solicitados Productos");
                var drawings = await _drawingService.GetAllDrawings();
                var products = _drawingService.GetProducts(drawings);
                _logger.LogInformation("Productos recuperados: " + products.Count);
                return Ok(products);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error al recuperar los productos: "+ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error al recuperar los productos." });

            }
        }

        [HttpGet("drawing/characters")]
        public async Task<ActionResult<List<CharacterListItem>>> DrawingCharacters()
        {
            try
            {
                _logger.LogInformation("Solicitados Personajes");
                var drawings = await _drawingService.GetAllDrawings();
                var characters = _drawingService.GetCharacters(drawings);
                _logger.LogInformation("Personajes recuperados: " + characters.Count);
                return Ok(characters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al recuperar los personajes: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al recuperar los personajes." });
            }
        }

        [HttpGet("drawing/models")]
        public async Task<ActionResult<List<string>>> DrawingModels()
        {
            try
            {
                _logger.LogInformation("Solicitados Modelos");
                var drawings = await _drawingService.GetAllDrawings();
                var models = _drawingService.GetModels(drawings);
                _logger.LogInformation("Modelos recuperados: " + models.Count);
                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al recuperar los modelos: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al recuperar los modelos." });
            }
        }
        #endregion

        #region Drawings Details
        [HttpGet("drawing/details/{id}")]
        public async Task<ActionResult<Drawing>> DrawingDetails(string id)
        {
            try
            {
                _logger.LogInformation($"Solicitados detalles públicos de dibujo \"{id}\"");
                var drawing = await _drawingService.FindDrawingById(id, true, updateViews: true, cache: false);
                if (drawing == null)
                {
                    _logger.LogWarning($"No se encontró ningún dibujo público \"{id}\"");
                    return NotFound(new { message = $"No se encontró ningún dibujo público \"{id}\"" });
                }
                return Ok(drawing);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al recuperar los detalles públicos del dibujo \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al recuperar los detalles públicos del dibujo \"{id}\"" });
            }
        }

        [HttpGet("drawing/full-details/{id}")]
        [Authorize]
        public async Task<ActionResult<Drawing>> DrawingFullDetails(string id)
        {
            try
            {
                _logger.LogInformation($"Solicitados detalles de dibujo \"{id}\"");
                var drawing = await _drawingService.FindDrawingById(id, false, updateViews: false, cache: false);
                if (drawing == null)
                {
                    _logger.LogWarning($"No se encontró ningún dibujo \"{id}\"");
                    return NotFound(new { message = $"No se encontró ningún dibujo \"{id}\"" });
                }
                return Ok(drawing);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al recuperar los detalles del dibujo \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al recuperar los detalles del dibujo \"{id}\"" });
            }
        }
        #endregion

        #region Drawing Filters
        [HttpPost("drawing/filter")]
        public async Task<ActionResult<DrawingFilterResultsResponse>> DrawingFilter([FromBody] DrawingFilter filters)
        {
            try
            {
                _logger.LogInformation($"Filtrando dibujos públicos");
                var allDrawings = await _drawingService.GetAllDrawings();
                var allCollections = await _drawingService.GetAllCollections(allDrawings);
                filters.OnlyVisible = true;
                var results = await _drawingService.FilterDrawingsGivenList(filters, allDrawings, allCollections);
                _logger.LogInformation($"Dibujos públicos filtrados: {results.TotalDrawings.Count}");
                return Ok(new DrawingFilterResultsResponse(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al filtrar dibujos públicos: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al filtrar dibujos públicos" });
            }
        }

        [HttpPost("drawing/full-filter")]
        [Authorize]
        public async Task<ActionResult<DrawingFilterResultsResponse>> DrawingFullFilter([FromBody] DrawingFilter filters)
        {
            try
            {
                _logger.LogInformation($"Filtrando dibujos");
                var allDrawings = await _drawingService.GetAllDrawings();
                var allCollections = await _drawingService.GetAllCollections(allDrawings);
                filters.OnlyVisible = false;
                var results = await _drawingService.FilterDrawingsGivenList(filters, allDrawings, allCollections);
                _logger.LogInformation($"Dibujos filtrados: {results.TotalDrawings.Count}");
                return Ok(new DrawingFilterResultsResponse(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al filtrar dibujos: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al filtrar dibujos" });
            }
        }
        #endregion

        #region Drawing Interactions
        [HttpPost("drawing/cheer")]
        public async Task<ActionResult> DrawingCheer([FromBody] string id)
        {
            try
            {
                _logger.LogInformation($"Recibido like para dibujo \"{id}\"");
                await _drawingService.UpdateLikes(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al recibir like para dibujo \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al recibir like para dibujo \"{id}\"" });
            }
        }

        [HttpPost("drawing/vote/{id}")]
        public async Task<ActionResult<VoteSubmittedModel>> DrawingVote(string id, [FromBody] int score)
        {
            try
            {
                _logger.LogInformation($"Recibido voto para dibujo \"{id}\" ({score})");
                var results = await _drawingService.Vote(id, score);
                _logger.LogInformation($"Nuevos resultados para \"{id}\" ({results.NewScoreHuman}) [{results.NewVotes} votos]");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al recibir voto para dibujo \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al recibir voto para dibujo \"{id}\"" });
            }
        }
        #endregion

        #region Drawing Form
        [Authorize]
        [HttpPost("drawing/save/{id}")]
        public async Task<ActionResult<Drawing>> DrawingSave(string id, [FromBody] SaveDrawingRequest request)
        {
            try
            {
                _logger.LogInformation($"Guardando dibujo \"{id}\"");
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar dibujo \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al guardar dibujo \"{id}\"" });
            }
        }

        [HttpGet("drawing/exist/{id}")]
        public async Task<ActionResult<bool>> DrawingExist(string id)
        {
            try
            {
                _logger.LogInformation($"Comprobando si existe dibujo \"{id}\"");
                var drawing = await _drawingService.FindDrawingById(id, false);
                var exists = drawing != null;
                _logger.LogInformation($"Existe dibujo \"{id}\": {(exists ? "Sí" : "No")}");
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al comprobar dibujo \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al comprobar dibujo \"{id}\"" });
            }
        }

        [Authorize]
        [HttpPost("drawing/check/blob")]
        public async Task<ActionResult<CheckAzurePathResponse>> DrawingCheckBlob([FromBody] CheckAzurePathRequest request)
        {
            try
            {
                _logger.LogInformation($"Comprobando si existe blob de Azure \"{request.Id}\"");
                var existe = await _drawingService.ExistsBlob(request.Id);
                if (!existe)
                {
                    _logger.LogWarning($"No existe el blob de Azure \"{request.Id}\"");
                }

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
                _logger.LogError($"Error al comprobar si existe blob de Azure \"{request.Id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al comprobar si existe blob de Azure \"{request.Id}\"" });
            }
        }

        [HttpPost("drawing/upload/blob")]
        [Authorize]
        public async Task<ActionResult<UploadAzureImageResponse>> DrawingUploadBlob([FromForm] IFormFile image, [FromForm] int size, [FromForm] string path)
        {
            try
            {
                _logger.LogInformation($"Subiendo fichero a Azure \"{image.FileName}\"");
                if (image == null || image.Length == 0)
                {
                    //return new UploadAzureImageResponse()
                    //{
                    //    Ok = false,
                    //    Error = "No se ha proporcionado ninguna imagen",
                    //};
                    _logger.LogWarning("No se ha proporcionado ningún fichero");
                    return BadRequest(new { message = $"No se ha proporcinoado ningún fichero" });
                }

                var blobLocationThumbnail = _drawingService.CrearThumbnailName(path);
                await UploadImage(image, blobLocationThumbnail, size);
                _logger.LogInformation($"Subida imagen de Thumbnail a \"{blobLocationThumbnail}\"");
                await UploadImage(image, path, 0);
                _logger.LogInformation($"Subida imagen a \"{path}\"");

                var urlBase = _drawingService.GetAzureUrlBase();
                var url = urlBase + path;
                var url_tn = urlBase + blobLocationThumbnail;

                return Ok(new UploadAzureImageResponse()
                {
                    Ok = true,
                    Error = "",
                    Url = url,
                    UrlThumbnail = url_tn,
                    PathThumbnail = blobLocationThumbnail
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al subir blob de Azure: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al subir blob de Azure" });
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
        #endregion

        #region Collection Details
        [HttpGet("collection/details/{id}")]
        public async Task<ActionResult<CollectionResponse>> CollectionDetails(string id)
        {
            try
            {
                _logger.LogInformation($"Solicitados detalles públicos de colección \"{id}\"");
                var drawings = await _drawingService.GetAllDrawings();
                var collection = await _drawingService.FindCollectionById(id, drawings);
                if (collection == null)
                {
                    _logger.LogWarning($"No se encontró ninguna colección \"{id}\"");
                    return NotFound(new { message = $"No se encontró ninguna colección \"{id}\"" });
                }
                var publicCollection = FilterCollectionResponsePublic(new CollectionResponse(collection));
                return Ok(publicCollection);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al recuperar los detalles públicos de la colección \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al recuperar los detalles públicos de la colección \"{id}\"" });
            }
        }

        [HttpGet("collection/full-details/{id}")]
        [Authorize]
        public async Task<ActionResult<CollectionResponse>> CollectionFullDetails(string id)
        {
            try
            {
                _logger.LogInformation($"Solicitados detalles de colección \"{id}\"");
                var drawings = await _drawingService.GetAllDrawings();
                var collection = await _drawingService.FindCollectionById(id, drawings);
                if (collection == null)
                {
                    _logger.LogWarning($"No se encontró ninguna colección \"{id}\"");
                    return NotFound(new { message = $"No se encontró ninguna colección \"{id}\"" });
                }
                return Ok(new CollectionResponse(collection));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al recuperar los detalles de la colección \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al recuperar los detalles de la colección \"{id}\"" });
            }
        }
        #endregion

        #region Collection List
        [HttpGet("collections")]
        public async Task<ActionResult<List<CollectionResponse>>> Collections()
        {
            try
            {
                _logger.LogInformation("Solicitadas Colecciones Públicas");
                var drawings = await _drawingService.GetAllDrawings();
                var collections = (await _drawingService.GetAllCollections(drawings)).Select(x => new CollectionResponse(x)).ToList();
                var publicCollections = new List<CollectionResponse>();
                foreach (var collection in collections)
                {
                    publicCollections.Add(FilterCollectionResponsePublic(collection));
                }
                _logger.LogInformation("Colecciones Públicas: "+publicCollections.Count);
                return Ok(publicCollections);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al recuperar las colecciones públicas: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al recuperar las colecciones públicas." });
            }
        }

        [HttpGet("collections/full")]
        [Authorize]
        public async Task<ActionResult<List<CollectionResponse>>> CollectionsFull()
        {
            try
            {
                _logger.LogInformation("Solicitadas Colecciones");
                var drawings = await _drawingService.GetAllDrawings();
                var collections = (await _drawingService.GetAllCollections(drawings)).Select(x => new CollectionResponse(x)).ToList();
                _logger.LogInformation("Colecciones: " + collections.Count);
                return Ok(collections);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al recuperar las colecciones: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al recuperar las colecciones." });
            }
        }

        private CollectionResponse FilterCollectionResponsePublic(CollectionResponse collection)
        {
            collection.Drawings = collection.Drawings.Where(x => x.Visible).ToList();
            collection.DrawingsId = collection.Drawings.Select(x => x.Id).ToList();
            return collection;
        }
        #endregion

        #region Collection Form

        [HttpGet("collection/exist/{id}")]
        public async Task<ActionResult<bool>> CollectionExistId(string id)
        {
            try
            {
                _logger.LogInformation($"Comprobando si existe colección \"{id}\"");
                var drawings = await _drawingService.GetAllDrawings();
                var collection = await _drawingService.FindCollectionById(id, drawings);
                var exists = collection != null;
                _logger.LogInformation($"Existe colección \"{id}\": {(exists ? "Sí" : "No")}");
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al comprobar colección \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al comprobar colección \"{id}\"" });
            }
        }

        [HttpPost("collection/save/{id}")]
        [Authorize]
        public async Task<ActionResult<CollectionResponse>> CollectionSave(string id, [FromBody] SaveCollectionRequest model)
        {
            try
            {
                _logger.LogInformation($"Guardando colección \"{model.Id}\"");
                var collection = new Collection()
                {
                    Id = model.Id,
                    Description = model.Description,
                    Name = model.Name,
                    Order = model.Order
                };
                if (String.IsNullOrEmpty(collection.Id))
                {
                    _logger.LogWarning("No se ha proporcionado un ID correcto para la colección");
                    return BadRequest(new { message = "No se ha proporcionado un ID correcto para la colección" });
                }
                collection.DrawingsReferences = await _drawingService.SetDrawingsReferences(model.DrawingsIds);

                var drawings = await _drawingService.GetAllDrawings();
                Collection result = await _drawingService.AddAsync(collection, drawings);
                _logger.LogInformation($"Guardada colección \"{model.Id}\" con éxito");
                _drawingService.CleanAllCache();
                return new CollectionResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar colección \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al guardar colección \"{id}\"" });
            }
        }

        [HttpPost("collection/delete")]
        [Authorize]
        public async Task<ActionResult<bool>> CollectionDelete([FromBody] string id)
        {
            try
            {
                _logger.LogInformation($"Eliminando colección \"{id}\"");
                await _drawingService.RemoveCollection(id);
                _logger.LogInformation($"Colección \"{id}\" eliminada con éxito");
                _drawingService.CleanAllCache();
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar colección \"{id}\": " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = $"Error al eliminar colección \"{id}\"" });
            }
        }
        #endregion
    }
}
