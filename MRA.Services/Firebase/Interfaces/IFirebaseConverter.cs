using Google.Cloud.Firestore;
using MRA.Services.Firebase.Documents;
using MRA.Services.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Interfaces
{
    internal interface IFirebaseConverter<M, D>
    {
        M ConvertToModel(D document);

        D ConvertToDocument(M model);
    }
}
