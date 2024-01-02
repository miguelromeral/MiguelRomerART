using Google.Cloud.Firestore;
using Google.Protobuf.Compiler;
using MRA.Services.Firebase.Documents;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Converters
{
    internal class InspirationFirebaseConverter : IFirebaseConverter<Inspiration, InspirationDocument>
    {
        public Inspiration ConvertToModel(InspirationDocument drawingDocument)
        {
            return new Inspiration
            {
                Id = drawingDocument.Id,
                Name = drawingDocument.name,
                Instagram = drawingDocument.instagram,
                Type = drawingDocument.type,
                Twitter = drawingDocument.twitter,
            };
        }

        public InspirationDocument ConvertToDocument(Inspiration drawing)
        {
            return new InspirationDocument
            {
                Id = drawing.Id,
                name = drawing.Name,
                instagram = drawing.Instagram,
                twitter = drawing.Twitter,
                type = drawing.Type,
            };
        }
    }
}
