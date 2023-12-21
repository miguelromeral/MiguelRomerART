using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Interfaces
{
    public interface IFirestoreService
    {
        Task<List<Shoe>> GetAll();
    }
}
