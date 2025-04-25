using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;

namespace MRA.Services.Models.Drawings;

public interface IDrawingService
{
    Task<IEnumerable<DrawingModel>> GetAllDrawingsAsync(bool onlyIfVisible);
    Task<DrawingModel> FindDrawingAsync(string id, bool onlyIfVisible, bool updateViews);
    Task<bool> ExistsDrawing(string id);

    Task<IEnumerable<ProductListItem>> GetProductsAsync();
    Task<IEnumerable<CharacterListItem>> GetCharactersAsync();
    Task<IEnumerable<ModelListItem>> GetModelsAsync();

    Task<DrawingModel> SaveDrawingAsync(DrawingModel model);
    Task<DrawingModel> UpdateViewsAsync(string id);
    Task<DrawingModel> UpdateLikesAsync(string id);
    Task<VoteSubmittedModel> VoteDrawingAsync(string documentId, int score);

    IEnumerable<string> DeleteAndAdjustTags(IEnumerable<string> tags);
}
