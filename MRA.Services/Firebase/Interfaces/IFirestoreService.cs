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
        Task<List<Drawing>> GetAll();
        Task<List<Inspiration>> GetAllInspirations();
        Task<List<Collection>> GetAllCollections();
        Task<List<Collection>> GetAllCollectionsOrderPositive();
        Task<Drawing> AddAsync(Drawing document);
        Task<Collection> AddAsync(Collection document);
        Task<List<Drawing>> Filter(DrawingFilter filter);
        Task<FilterResults> FilterGivenList(DrawingFilter filter, List<Drawing> drawings, List<Collection> collections);
        Task<Drawing> FindDrawingById(string documentId, bool updateViews);
        Task<Collection> FindCollectionById(string documentId);
        Task UpdateLikes(string documentId);
        Task<VoteSubmittedModel> Vote(string documentId, int score);
        DocumentReference GetDbDocumentDrawing(string id);
        Task UpdateViews(string documentId);
        Task RemoveCollection(string id);
        Task<List<DocumentReference>> SetDrawingsReferences(string[] ids);
    }
}
