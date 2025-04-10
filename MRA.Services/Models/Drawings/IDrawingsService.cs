
using MRA.DTO.Firebase.Models;
using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art.Select;

namespace MRA.Services.Models.Drawings;

public interface IDrawingService
{
    Task<IEnumerable<DrawingModel>> GetAllDrawingsAsync(bool onlyIfVisible);
    Task<DrawingModel> FindDrawingAsync(string id, bool onlyIfVisible, bool updateViews);
    Task<bool> ExistsDrawing(string id);

    Task<IEnumerable<ProductListItem>> GetProductsAsync();
    Task<IEnumerable<CharacterListItem>> GetCharactersAsync();
    Task<IEnumerable<string>> GetModelsAsync();

    Task<bool> SaveDrawingAsync(string id, DrawingModel model);
    Task<bool> UpdateViewsAsync(string id);
    Task<bool> UpdateLikesAsync(string id);
    Task<VoteSubmittedModel> VoteDrawingAsync(string documentId, int score);

    void SetAutomaticTags(ref DrawingModel document);
    IEnumerable<string> DeleteAndAdjustTags(IEnumerable<string> tags);
}
