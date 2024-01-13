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
        private readonly string _urlBase;

        public DrawingFirebaseConverter(string urlBase)
        {
            _urlBase = urlBase;
        }

        public Drawing ConvertToModel(DrawingDocument drawingDocument)
        {
            return new Drawing
            {
                Id = drawingDocument.Id,
                Path = drawingDocument.path,
                Type = drawingDocument.type,
                Title = drawingDocument.title,
                Name = drawingDocument.name,
                Date = drawingDocument.date,
                Time = drawingDocument.time ?? 0,
                ProductType = drawingDocument.product_type,
                ProductName = drawingDocument.product_name,
                Comment = drawingDocument.comment,
                CommentPros = drawingDocument.comment_pros,
                CommentCons = drawingDocument.comment_cons,
                Views = drawingDocument.views,
                Likes = drawingDocument.likes,
                ModelName = drawingDocument.model_name,
                UrlBase = _urlBase,
                Favorite = drawingDocument.favorite,
                ReferenceUrl = drawingDocument.reference_url,
                PathThumbnail = drawingDocument.path_thumbnail,
                Software = drawingDocument.software,
                Paper = drawingDocument.paper,
                SpotifyUrl = drawingDocument.spotify_url,
                Tags = drawingDocument.tags ?? new List<string>()
            };
        }

        public DrawingDocument ConvertToDocument(Drawing drawing)
        {
            return new DrawingDocument
            {
                Id = drawing.Id,
                path = drawing.Path ?? "",
                type = drawing.Type,
                title = drawing.Title ?? "",
                date = drawing.Date,
                time = drawing.Time,
                name = drawing.Name ?? "",
                product_type = drawing.ProductType,
                product_name = drawing.ProductName ?? "",
                comment = drawing.Comment ?? "",
                comment_cons = drawing.CommentCons ?? "",
                comment_pros = drawing.CommentPros ?? "",
                views = drawing.Views,
                likes = drawing.Likes,
                model_name = drawing.ModelName ?? "",
                favorite = drawing.Favorite,
                reference_url = drawing.ReferenceUrl ?? "",
                path_thumbnail = drawing.PathThumbnail,
                software = drawing.Software,
                paper = drawing.Paper,
                spotify_url = drawing.SpotifyUrl,
                tags = drawing.Tags
            };
        }
    }
}
