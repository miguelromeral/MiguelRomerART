using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art;

namespace MRA.Services
{
    public interface IAppService
    {
        string GetAzureUrlBase();
        Task<IEnumerable<DrawingModel>> GetAllDrawings(bool onlyIfVisible, bool cache = true);
        Task<IEnumerable<CollectionModel>> GetAllCollectionsAsync(bool onlyIfVisible, bool cache = true);

        Task<bool> ExistsBlob(string rutaBlob);
        Task<CollectionModel> FindCollectionByIdAsync(string collectionId, bool onlyIfVisible, bool cache = true);
        Task<DrawingModel> FindDrawingByIdAsync(string drawingId, bool onlyIfVisible, bool updateViews = false, bool cache = true);

        Task<FilterResults> FilterDrawingsAsync(DrawingFilter filter);

        IEnumerable<DrawingModel> CalculatePopularityOfListDrawings(IEnumerable<DrawingModel> drawings);

        Task RedimensionarYGuardarEnAzureStorage(string rutaEntrada, string nombreBlob, int anchoDeseado);
        Task RedimensionarYGuardarEnAzureStorage(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado);
        string CrearThumbnailName(string rutaImagen);
        //Task<List<DocumentReference>> SetDrawingsReferences(string[] ids);
        void CleanAllCache();
    }
}
