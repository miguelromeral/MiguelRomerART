using Google.Cloud.Firestore;
using MRA.DTO.Firebase.Documents;
using MRA.DTO.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Firebase.Interfaces
{
    internal interface IFirebaseConverter<M, D>
    {
        M ConvertToModel(D document);

        D ConvertToDocument(M model);
    }
}
