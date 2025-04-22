using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.DTO.Exceptions;
using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Services;
using MRA.Services.Models.Drawings;
using MRA.Services.Storage;
using MRA.WebApi.Models.Responses;

namespace MRA.WebApi.Controllers.Art;

[ApiController]
[Route("api/art/drawing")]
public class DrawingController : Controller
{
    private readonly IAppService _appService;
    private readonly IDrawingService _drawingService;
    private readonly IStorageService _storageService;
    private readonly ILogger<DrawingController> _logger;

    public DrawingController(
        ILogger<DrawingController> logger,
        IAppService appService,
        IStorageService storageService,
        IDrawingService drawingService)
    {
        _logger = logger;
        _appService = appService;
        _drawingService = drawingService;
        _storageService = storageService;
    }

    [HttpGet("products")]
    public async Task<ActionResult<List<ProductListItem>>> Products()
    {
        try
        {
            _logger.LogInformation("Solicitados Productos");
            var products = await _drawingService.GetProductsAsync();
            _logger.LogInformation($"Productos recuperados: {products.Count()}");
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al recuperar los productos: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al recuperar los productos." });

        }
    }

    [HttpGet("characters")]
    public async Task<ActionResult<List<CharacterListItem>>> Characters()
    {
        try
        {
            _logger.LogInformation("Solicitados Personajes");
            var characters = await _drawingService.GetCharactersAsync();
            _logger.LogInformation("Personajes recuperados: " + characters.Count());
            return Ok(characters);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al recuperar los personajes: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al recuperar los personajes." });
        }
    }

    [HttpGet("models")]
    public async Task<ActionResult<List<string>>> Models()
    {
        try
        {
            _logger.LogInformation("Solicitados Modelos");
            var models = await _drawingService.GetModelsAsync();
            _logger.LogInformation("Modelos recuperados: '{Count}'", models.Count());
            return Ok(models.Select(x => x.ModelName));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al recuperar los modelos");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error al recuperar los modelos." });
        }
    }


    [HttpGet("details/{id}")]
    public async Task<ActionResult<DrawingModel>> Details(string id)
    {
        return await FetchDrawingDetails(id, true);
    }

    [Authorize]
    [HttpGet("full-details/{id}")]
    public async Task<ActionResult<DrawingModel>> FullDetails(string id)
    {
        return await FetchDrawingDetails(id, false);
    }

    private async Task<ActionResult<DrawingModel>> FetchDrawingDetails(string id, bool onlyIfVisible)
    {
        try
        {
            _logger.LogInformation($"Solicitados detalles de dibujo \"{id}\"");
            var drawing = await _appService.FindDrawingByIdAsync(id, onlyIfVisible: onlyIfVisible, updateViews: false, cache: false);
            return Ok(drawing);
        }
        catch (DrawingNotFoundException dnf)
        {
            _logger.LogWarning($"No se encontró ningún dibujo \"{id}\"");
            return NotFound(new { message = $"No drawing found with ID \"{id}\"" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al recuperar los detalles del dibujo \"{id}\": " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = $"Error when fetching drawing with ID \"{id}\"" });
        }
    }


    [HttpPost("filter")]
    public async Task<ActionResult<FilterResults>> Filter([FromBody] DrawingFilter filters)
    {
        filters.OnlyVisible = true;
        return await FilteringDrawings(filters);
    }

    [Authorize]
    [HttpPost("full-filter")]
    public async Task<ActionResult<FilterResults>> FullFilter([FromBody] DrawingFilter filters)
    {
        filters.OnlyVisible = false;
        return await FilteringDrawings(filters);
    }

    private async Task<ActionResult<FilterResults>> FilteringDrawings(DrawingFilter filters)
    {
        try
        {
            _logger.LogInformation($"Filtering drawings");
            var results = await _appService.FilterDrawingsAsync(filters);
            _logger.LogInformation("Dibujos filtrados: {Count}", results.TotalDrawings.Count());
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error when filtering drawings.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = $"Error when filtering drawings" });
        }
    }

    [HttpPost("cheer")]
    public async Task<ActionResult> Cheer([FromBody] string id)
    {
        try
        {
            _logger.LogInformation($"Recibido like para dibujo \"{id}\"");
            return Ok(await _drawingService.UpdateLikesAsync(id));
        }
        catch (DrawingNotFoundException dnf)
        {
            _logger.LogWarning($"No se encontró ningún dibujo \"{id}\"");
            return NotFound(new { message = $"No drawing found with ID \"{id}\"" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al recibir like para dibujo \"{id}\": " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = $"Error when liking drawing with ID \"{id}\"" });
        }
    }

    [HttpPost("vote/{id}")]
    public async Task<ActionResult<VoteSubmittedModel>> Vote(string id, [FromBody] int score)
    {
        try
        {
            _logger.LogInformation($"Recibido voto para dibujo \"{id}\" ({score})");
            var results = await _drawingService.VoteDrawingAsync(id, score);
            _logger.LogInformation($"Nuevos resultados para \"{id}\" ({results.NewScoreHuman}) [{results.NewVotes} votos]");
            return Ok(results);
        }
        catch (DrawingNotFoundException dnf)
        {
            _logger.LogWarning($"No se encontró ningún dibujo \"{id}\"");
            return NotFound(new { message = $"No drawing found with ID \"{id}\"" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al recibir voto para dibujo \"{id}\": " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = $"Error when receiving vote with ID \"{id}\"" });
        }
    }

    [Authorize]
    [HttpPost("save/{id}")]
    public async Task<ActionResult<DrawingModel>> Save(string id, [FromBody] SaveDrawingRequest request)
    {
        try
        {
            _logger.LogInformation("Guardando dibujo '{Id}'", id);
            var drawing = new DrawingModel()
            {
                ListComments = request.ListComments ?? [],
                ListCommentsStyle = request.ListCommentsStyle ?? [],
                ListCommentsPros = request.ListCommentsPros ?? [],
                ListCommentsCons = request.ListCommentsCons ?? [],
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
                Tags = request.TagsText?.Split(DrawingTagManager.TAG_SEPARATOR) ?? [],
                Time = request.Time ?? 0,
                Title = request.Title ?? string.Empty,
                Type = request.Type,
                InstagramUrl = request.InstagramUrl,
                TwitterUrl = request.BlueskyUrl,
                Visible = request.Visible
            };

            await _drawingService.SaveDrawingAsync(drawing);
            _appService.Clear();
            return Ok(drawing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al guardar dibujo \"{id}\": " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = $"Error al guardar dibujo \"{id}\"" });
        }
    }

    [HttpGet("exist/{id}")]
    public async Task<ActionResult<bool>> Exist(string id)
    {
        try
        {
            _logger.LogInformation($"Comprobando si existe dibujo \"{id}\"");
            var exists = await _drawingService.ExistsDrawing(id);
            _logger.LogInformation($"Existe dibujo \"{id}\": {(exists ? "Sí" : "No")}");
            return Ok(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al comprobar dibujo \"{id}\": " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = $"Error when checking drawing with ID \"{id}\"" });
        }
    }

    [Authorize]
    [HttpPost("check/blob")]
    public async Task<ActionResult<CheckAzurePathResponse>> CheckBlob([FromBody] CheckAzurePathRequest request)
    {
        try
        {
            _logger.LogInformation($"Comprobando si existe blob de Azure \"{request.Id}\"");
            var existe = await _storageService.ExistsBlob(request.Id);
            if (!existe)
            {
                _logger.LogWarning($"No existe el blob de Azure \"{request.Id}\"");
            }

            var blobLocationThumbnail = _storageService.CrearThumbnailName(request.Id);

            var urlBase = _storageService.GetBlobURL();
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
                new { message = $"Error when checking blob \"{request.Id}\"" });
        }
    }

    [Authorize]
    [HttpPost("upload/blob")]
    public async Task<ActionResult<UploadAzureImageResponse>> UploadBlob(IFormFile image, [FromForm] int size, [FromForm] string path)
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

            var blobLocationThumbnail = _storageService.CrearThumbnailName(path);
            await UploadImage(image, blobLocationThumbnail, size);
            _logger.LogInformation($"Subida imagen de Thumbnail a \"{blobLocationThumbnail}\"");
            await UploadImage(image, path, 0);
            _logger.LogInformation($"Subida imagen a \"{path}\"");

            var urlBase = _storageService.GetBlobURL();
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
                new { message = $"Error when uploading blob" });
        }
    }

    private async Task UploadImage(IFormFile azureImage, string path, int azureImageThumbnailSize)
    {
        using (var imageStream = new MemoryStream())
        {
            await azureImage.CopyToAsync(imageStream);
            imageStream.Position = 0;

            await _storageService.ResizeAndSave(imageStream, path, azureImageThumbnailSize);
        }
    }
}
