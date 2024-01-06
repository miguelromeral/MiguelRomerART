using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MRA.Services.Firebase.Models;
using MRA.Web.Models.Art;

namespace MRA.Services.Firebase.Interfaces
{
    public interface IFirestoreService
    {
        Task<List<Drawing>> GetAll();
        Task<List<Inspiration>> GetAllInspirations();
        Task<List<Collection>> GetAllCollections();
        Task<Drawing> AddAsync(Drawing document);
        Task<List<Drawing>> Filter(FilterDrawingModel filter);
        Task<Drawing> FindDrawingById(string documentId);
        Task UpdateLikes(string documentId);
    }
}
