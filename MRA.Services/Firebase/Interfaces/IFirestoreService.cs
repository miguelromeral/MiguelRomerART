using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.Firebase.Models;

namespace MRA.Services.Firebase.Interfaces
{
    public interface IFirestoreService
    {
        Task<List<Drawing>> GetDrawingsAsync();
        Task<List<Inspiration>> GetInspirationsAsync();
        Task<List<Collection>> GetCollectionsAsync(List<Drawing> drawings);
        Task<Drawing> AddDrawingAsync(Drawing document);
        Task<Collection> AddCollectionAsync(Collection document, List<Drawing> drawings);
        //Task<List<Drawing>> Filter(DrawingFilter filter);
        Task<FilterResults> FilterGivenList(DrawingFilter filter, List<Drawing> drawings, List<Collection> collections);
        Task<Drawing> FindDrawingByIdAsync(string documentId, bool updateViews);
        Task<Collection> FindCollectionByIdAsync(string documentId, List<Drawing> drawings);
        Task<bool> UpdateLikesAsync(string documentId);
        Task<VoteSubmittedModel> VoteAsync(string documentId, int score);
        //DocumentReference GetDbDocumentDrawing(string id);
        Task<bool> UpdateViewsAsync(string documentId);
        Task<Google.Cloud.Firestore.WriteResult> RemoveCollectionAsync(string id);
        Task<List<DocumentReference>> SetDrawingsReferencesAsync(string[] ids);
    }
}
