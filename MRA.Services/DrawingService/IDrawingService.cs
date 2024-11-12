using Google.Cloud.Firestore;
using MRA.DTO.Firebase.Models;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services
{
    public interface IDrawingService
    {
        Task<List<Drawing>> GetAllDrawings();
        string GetAzureUrlBase();
        Task<List<Inspiration>> GetAllInspirations();
        Task<List<Collection>> GetAllCollections(List<Drawing> drawings, bool cache = true);
        Task<Collection> FindCollectionById(string documentId, List<Drawing> drawings, bool cache = true);
        Task RemoveCollection(string id);
        Task<FilterResults> FilterDrawingsGivenList(DrawingFilter filter, List<Drawing> drawings, List<Collection> collections);
        //Task<List<Drawing>> FilterDrawings(DrawingFilter filter);
        Task<Drawing> FindDrawingById(string documentId, bool onlyIfVisible, bool updateViews = false, bool cache = true);
        Task<bool> UpdateViews(string documentId);
        Task<bool> UpdateLikes(string documentId);
        Task<VoteSubmittedModel> Vote(string documentId, int score);
        Task<bool> ExistsBlob(string rutaBlob);
        Task<Drawing> AddAsync(Drawing document);
        Task<Collection> AddAsync(Collection document, List<Drawing> drawings);
        Task RedimensionarYGuardarEnAzureStorage(string rutaEntrada, string nombreBlob, int anchoDeseado);
        Task RedimensionarYGuardarEnAzureStorage(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado);
        string CrearThumbnailName(string rutaImagen);
        //DocumentReference GetDbDocumentDrawing(string id);
        Task<List<DocumentReference>> SetDrawingsReferences(string[] ids);
        List<ProductListItem> GetProducts(List<Drawing> drawings);
        List<CharacterListItem> GetCharacters(List<Drawing> drawings);
        List<string> GetModels(List<Drawing> drawings);

        void CleanAllCache();
    }
}
