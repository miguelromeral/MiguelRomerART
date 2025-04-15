using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Services;
using MRA.WebApi.Models.Requests;
using MRA.WebApi.Models.Responses;
using MRA.Services.Models.Drawings;
using MRA.Services.Models.Collections;
using MRA.DTO.Exceptions;
using MRA.DTO.Models;
using MRA.Services.Storage;

namespace MRA.WebApi.Controllers;

[ApiController]
[Route("api/art")]
public class ArtController : Controller
{

    private readonly IAppService _appService;
    private readonly IStorageService _storageService;
    private readonly IDrawingService _drawingService;
    private readonly ICollectionService _collectionService;
    private readonly ILogger<ArtController> _logger;

    public ArtController(
        ILogger<ArtController> logger, 
        IAppService appService,
        IDrawingService drawingService,
        IStorageService storageService,
        ICollectionService collectionService
        )
    {
        _logger = logger;
        _appService = appService;
        _drawingService = drawingService;
        _storageService = storageService;
        _collectionService = collectionService;
    }

    #region Collection List
    [HttpGet("collections")]
    public async Task<ActionResult<IEnumerable<CollectionResponse>>> Collections()
    {
        return await FetchCollectionList(true);
    }

    [HttpGet("collections/full")]
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
    #endregion
}
