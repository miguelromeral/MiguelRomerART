using MRA.Infrastructure.Settings;
using MRA.DTO.Exceptions;
using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Services.Models.Documents;
using MRA.DTO.ViewModels.Art;
using MRA.Infrastructure.Database.Providers.Interfaces;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.DTO.Mapper;

namespace MRA.Services.Models.Drawings;

public class DrawingService : DocumentModelService<DrawingModel, IDrawingDocument>, IDrawingService
{
    private readonly DrawingTagManager _drawingTagManager;

    public DrawingService(
        AppSettings appConfig,
        IDocumentsDatabase db)
        : base(collectionName: appConfig.Database.Collections.Drawings, new DrawingMapper(appConfig), db)
    {
        _drawingTagManager = new DrawingTagManager(appConfig);
    }

    public async Task<IEnumerable<DrawingModel>> GetAllDrawingsAsync(bool onlyIfVisible)
    {
        var drawings = await GetAllAsync();

        if (onlyIfVisible)
            drawings = drawings.Where(d => d.Visible);

        return drawings;
    }

    public async Task<DrawingModel> FindDrawingAsync(string id, bool onlyIfVisible, bool updateViews)
    {
        await CheckIfExistsDrawingAsync(id);

        var drawing = (updateViews ? await UpdateViewsAsync(id) : await FindAsync(id));

        if (onlyIfVisible && !drawing.Visible)
            throw new DrawingNotFoundException(id);

        return drawing;
    }

    public async Task<IEnumerable<ProductListItem>> GetProductsAsync()
    {
        var drawings = await GetAllAsync();
        return ProductListItem.GetProductsFromDrawings(drawings);
    }

    public async Task<IEnumerable<CharacterListItem>> GetCharactersAsync()
    {
        var drawings = await GetAllAsync();
        return CharacterListItem.GetCharactersFromDrawings(drawings);
    }

    public async Task<IEnumerable<ModelListItem>> GetModelsAsync()
    {
        var drawings = await GetAllAsync();
        return ModelListItem.GetModelsFromDrawings(drawings);
    }

    public async Task<DrawingModel> SaveDrawingAsync(DrawingModel model)
    {
        var drawingWithTags = _drawingTagManager.SetAutomaticTags(model);
        await SetAsync(model.Id, drawingWithTags);

        return drawingWithTags;
    }

    public async Task<DrawingModel> UpdateViewsAsync(string id)
    {
        var drawing = await FindAsync(id);
        drawing.Views++;

        await SetAsync(id, drawing);

        return drawing;
    }

    public async Task<DrawingModel> UpdateLikesAsync(string id)
    {
        await CheckIfExistsDrawingAsync(id);

        var drawing = await FindAsync(id);
        drawing.Likes++;

        await SetAsync(id, drawing);

        return drawing;
    }


    public async Task<VoteSubmittedModel> VoteDrawingAsync(string documentId, int score)
    {
        await CheckIfExistsDrawingAsync(documentId);

        var drawing = await FindAsync(documentId);
        drawing.UpdateDrawingScore(score);

        var success = await SetAsync(documentId, drawing);

        return new VoteSubmittedModel(drawing, success);
    }

    private async Task CheckIfExistsDrawingAsync(string id)
    {
        var exists = await ExistsAsync(id);
        if (!exists)
            throw new DrawingNotFoundException(id);
    }

    public async Task<bool> ExistsDrawing(string id)
    {
        return await ExistsAsync(id);
    }


    public IEnumerable<string> DeleteAndAdjustTags(IEnumerable<string> tags)
    {
        return _drawingTagManager.DeleteAndAdjustTags(tags);
    }
}
