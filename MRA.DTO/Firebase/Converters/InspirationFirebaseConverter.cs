using Google.Cloud.Firestore;
using Google.Protobuf.Compiler;
using MRA.DTO.Firebase.Documents;
using MRA.DTO.Firebase.Interfaces;
using MRA.DTO.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Firebase.Converters
{
    public class InspirationFirebaseConverter : IFirebaseConverter<Inspiration, InspirationDocument>
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
                YouTube = drawingDocument.youtube,
                Twitch = drawingDocument.twitch,
                Pinterest = drawingDocument.pinterest,
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
                youtube = drawing.YouTube,
                twitch = drawing.Twitch,
                pinterest = drawing.Pinterest,
            };
        }
    }
}
