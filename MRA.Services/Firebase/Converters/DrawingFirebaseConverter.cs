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
    internal class DrawingFirebaseConverter : IFirebaseConverter<Drawing, DrawingDocument>
    {
        public Drawing ConvertToModel(DrawingDocument drawingDocument)
        {
            return new Drawing
            {
                Id = drawingDocument.Id,
                Path = drawingDocument.path
            };
        }

        public DrawingDocument ConvertToDocument(Drawing drawing)
        {
            return new DrawingDocument
            {
                Id = drawing.Id,
                path = drawing.Path
            };
        }
    }
}
