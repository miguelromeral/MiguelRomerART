using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MRA.Services.Firebase.Models;

namespace MRA.Services.Firebase.Interfaces
{
    public interface IFirestoreService
    {
        Task<List<Drawing>> GetAll();
        Task<Drawing> FindDrawingById(string documentId);
    }
}
