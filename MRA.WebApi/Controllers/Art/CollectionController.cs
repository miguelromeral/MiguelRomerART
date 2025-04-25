using Microsoft.AspNetCore.Mvc;
using MRA.Services.Models.Collections;
using MRA.Services;
using Microsoft.AspNetCore.Authorization;
using MRA.WebApi.Models.Responses;
using MRA.WebApi.Models.Requests;
using MRA.WebApi.Models.Responses.Errors;
using MRA.DTO.Exceptions.Collections;

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
            return NotFound(new ErrorResponse(cnf.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessages.CollectionErrorMessages.FetchDetails.InternalServer(id));
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ErrorResponse(ErrorMessages.CollectionErrorMessages.FetchDetails.InternalServer(id)));
        }
    }

    [HttpGet("exist/{id}")]
    public async Task<ActionResult<bool>> Exists(string id)
    {
        try
        {
            var exists = await _collectionService.ExistsCollection(id);
            _logger.LogInformation("Existe colección '{Id}': {Exists}", id, (exists ? "Sí" : "No"));
            return Ok(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al comprobar colección '{Id}'.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ErrorResponse($"Error when checking collection \"{id}\""));
        }
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<CollectionResponse>>> List()
    {
        return await FetchCollectionList(true);
    }

    [HttpGet("full-list")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CollectionResponse>>> FullList()
    {
        return await FetchCollectionList(false);
    }

    private async Task<ActionResult<IEnumerable<CollectionResponse>>> FetchCollectionList(bool onlyIfVisible)
    {
        try
        {
            _logger.LogInformation("Fetching collections, only visible: {OnlyIfVisible}", (onlyIfVisible));
            var collections = await _appService.GetAllCollectionsAsync(onlyIfVisible: onlyIfVisible, cache: true);

            if (onlyIfVisible && collections.Any(c => c.Drawings.Any(d => !d.Visible)))
            {
                throw new VisibleCollectionRetrievedException();
            }

            _logger.LogInformation("{Count} collections found" , collections.Count());
            return Ok(collections.Select(c => new CollectionResponse(c)).ToList());
        }
        catch (VisibleCollectionRetrievedException vcr)
        {
            _logger.LogError(vcr, vcr.Message);
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new ErrorResponse(vcr.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessages.CollectionErrorMessages.FetchList.InternalServer);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse(ErrorMessages.CollectionErrorMessages.FetchList.InternalServer));
        }
    }

    [Authorize]
    [HttpPost("save/{id}")]
    public async Task<ActionResult<CollectionResponse>> Save(string id, [FromBody] SaveCollectionRequest model)
    {
        try
        {
            _logger.LogInformation("Saving collection '{Id}'", model.Id);
            var collection = model.GetModel();
            if (String.IsNullOrEmpty(collection.Id))
            {
                _logger.LogWarning(ErrorMessages.CollectionErrorMessages.Save.IdNotProvided);
                return BadRequest(new ErrorResponse(ErrorMessages.CollectionErrorMessages.Save.IdNotProvided));
            }

            await _collectionService.SaveCollectionAsync(id, collection);
            _logger.LogInformation("Saved collection '{Id}'", model.Id);
            _appService.Clear();
            return Ok(new CollectionResponse(collection));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessages.CollectionErrorMessages.Save.InternalServer(model.Id));
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse(ErrorMessages.CollectionErrorMessages.Save.InternalServer(model.Id)));
        }
    }

    [Authorize]
    [HttpPost("delete")]
    public async Task<ActionResult<bool>> Delete([FromBody] string id)
    {
        try
        {
            await _collectionService.DeleteCollection(id);
            _logger.LogInformation("Colección '{Id}' eliminada con éxito", id);
            _appService.Clear();
            return Ok(true);
        }
        catch (CollectionNotFoundException cnf)
        {
            _logger.LogWarning(cnf, cnf.Message);
            return NotFound(new ErrorResponse(cnf.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessages.CollectionErrorMessages.Delete.InternalServer(id));
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse(ErrorMessages.CollectionErrorMessages.Delete.InternalServer(id)));
        }
    }
}
