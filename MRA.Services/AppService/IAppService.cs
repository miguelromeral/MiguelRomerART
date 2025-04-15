using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art;

namespace MRA.Services
{
    public interface IAppService
    {
        Task<IEnumerable<DrawingModel>> GetAllDrawings(bool onlyIfVisible, bool cache = true);
        Task<IEnumerable<CollectionModel>> GetAllCollectionsAsync(bool onlyIfVisible, bool cache = true);

        Task<CollectionModel> FindCollectionByIdAsync(string collectionId, bool onlyIfVisible, bool cache = true);
        Task<DrawingModel> FindDrawingByIdAsync(string drawingId, bool onlyIfVisible, bool updateViews = false, bool cache = true);

        Task<FilterResults> FilterDrawingsAsync(DrawingFilter filter);

        IEnumerable<DrawingModel> CalculatePopularityOfListDrawings(IEnumerable<DrawingModel> drawings);

        
        void CleanAllCache();
    }
}
