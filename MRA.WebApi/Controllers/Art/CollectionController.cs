using Microsoft.AspNetCore.Mvc;
using MRA.Services.Models.Collections;
using MRA.Services.Models.Drawings;
using MRA.Services;
using Microsoft.AspNetCore.Authorization;
using MRA.DTO.Exceptions;
using MRA.WebApi.Models.Responses;
using MRA.DTO.Models;
using MRA.WebApi.Models.Requests;
using MRA.WebApi.Models.Responses.Errors;

namespace MRA.WebApi.Controllers.Art;

[ApiController]
[Route("api/art/collection")]
public class CollectionController : ControllerBase
{
    private readonly IAppService _appService;
    private readonly ICollectionService _collectionService;
    private readonly ILogger<CollectionController> _logger;

    public CollectionController(
        ILogger<CollectionController> logger,
        IAppService appService,
        ICollectionService collectionService
        )
    {
        _logger = logger;
        _appService = appService;
        _collectionService = collectionService;
    }


    [HttpGet("details/{id}")]
    public async Task<ActionResult<CollectionResponse>> Details(string id)
    {
        return await FetchCollectionDetails(id, true);
    }


    [Authorize]
    [HttpGet("full-details/{id}")]
    public async Task<ActionResult<CollectionResponse>> FullDetails(string id)
    {
        return await FetchCollectionDetails(id, false);
    }

    private async Task<ActionResult<CollectionResponse>> FetchCollectionDetails(string id, bool onlyIfVisible)
    {
        try
        {
            var collection = await _appService.FindCollectionByIdAsync(id, onlyIfVisible: onlyIfVisible, cache: true);
            return Ok(new CollectionResponse(collection));
        }
        catch (CollectionNotFoundException cnf)
        {
            _logger.LogWarning(cnf, cnf.Message);
            return NotFound(new NotFoundResponse(cnf.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when retrieving collection with ID '{Id}'.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new NotFoundResponse($"Error when retrieving collection with ID \"{id}\""));
        }
    }

    [HttpGet("exist/{id}")]
    public async Task<ActionResult<bool>> Exists(string id)
    {
        try
        {
            _logger.LogInformation($"Comprobando si existe colección \"{id}\"");
            var exists = await _collectionService.ExistsCollection(id);
            _logger.LogInformation($"Existe colección \"{id}\": {(exists ? "Sí" : "No")}");
            return Ok(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al comprobar colección \"{id}\": " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = $"Error when checking collection \"{id}\"" });
        }
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<CollectionResponse>>> Collections()
    {
        return await FetchCollectionList(true);
    }

    [HttpGet("full-list")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CollectionResponse>>> CollectionsFull()
    {
        return await FetchCollectionList(false);
    }

    private async Task<ActionResult<IEnumerable<CollectionResponse>>> FetchCollectionList(bool onlyIfVisible)
    {
        try
        {
            _logger.LogInformation("Solicitadas Colecciones");
            var collections = await _appService.GetAllCollectionsAsync(onlyIfVisible: onlyIfVisible, cache: true);
            _logger.LogInformation("Colecciones Públicas: " + collections.Count());
            return Ok(collections.Select(c => new CollectionResponse(c)));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al recuperar las colecciones: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error when fetching collections." });
        }
    }

    [Authorize]
    [HttpPost("save/{id}")]
    public async Task<ActionResult<CollectionResponse>> Save(string id, [FromBody] SaveCollectionRequest model)
    {
        try
        {
            _logger.LogInformation($"Guardando colección \"{model.Id}\"");
            var collection = new CollectionModel()
            {
                Id = model.Id,
                Description = model.Description,
                Name = model.Name,
                Order = model.Order,
                DrawingIds = model.DrawingsIds
            };
            if (String.IsNullOrEmpty(collection.Id))
            {
                _logger.LogWarning("No se ha proporcionado un ID correcto para la colección");
                return BadRequest(new { message = "No correct ID provided for the collection" });
            }

            await _collectionService.SaveCollectionAsync(id, collection);
            _logger.LogInformation($"Guardada colección \"{model.Id}\" con éxito");
            _appService.CleanAllCache();
            return new CollectionResponse(collection);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al guardar colección \"{id}\": " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = $"Error when saving collection \"{id}\"" });
        }
    }

    [Authorize]
    [HttpPost("delete")]
    public async Task<ActionResult<bool>> Delete([FromBody] string id)
    {
        try
        {
            _logger.LogInformation($"Eliminando colección \"{id}\"");
            await _collectionService.DeleteCollection(id);
            _logger.LogInformation($"Colección \"{id}\" eliminada con éxito");
            _appService.CleanAllCache();
            return Ok(true);
        }
        catch (CollectionNotFoundException cnf)
        {
            _logger.LogWarning($"No se encontró ninguna colección \"{id}\"");
            return NotFound(new { message = $"No collection found with ID \"{id}\"" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar colección \"{id}\": " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = $"Error when removing collection \"{id}\"" });
        }
    }
}
