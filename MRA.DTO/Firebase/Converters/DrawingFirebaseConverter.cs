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
    public class DrawingFirebaseConverter : IFirebaseConverter<Drawing, DrawingDocument>
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
                DateObject = MRA.DTO.Utilities.ConvertirStringADateTime(drawingDocument.date),
                DateHyphen = (drawingDocument.date ?? "").Replace("/", "-"),
                Time = drawingDocument.time ?? 0,
                ProductType = drawingDocument.product_type,
                ProductName = drawingDocument.product_name,
                ListComments = drawingDocument.list_comments ?? new List<string>(),
                ListCommentsStyle = drawingDocument.list_comments_style ?? new List<string>(),
                ListCommentsPros = drawingDocument.list_comments_pros ?? new List<string>(),
                ListCommentsCons = drawingDocument.list_comments_cons ?? new List<string>(),
                Filter = drawingDocument.filter,
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
                InstagramUrl = drawingDocument.instagram_url,
                TwitterUrl = drawingDocument.twitter_url,
                ScorePopular = drawingDocument.score_popular,
                ScoreCritic = drawingDocument.score_critic,
                VotesPopular = drawingDocument.votes_popular,
                Tags = drawingDocument.tags ?? new List<string>(),
                Visible = drawingDocument.visible ?? true,
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
                drawingAt = drawing.DateObject.ToUniversalTime(),
                time = drawing.Time,
                name = drawing.Name ?? "",
                product_type = drawing.ProductType,
                product_name = drawing.ProductName ?? "",
                //comment = drawing.Comment ?? "",
                list_comments = drawing.ListComments ?? new List<string>(),
                //comment_pros = drawing.CommentPros ?? "",
                list_comments_pros = drawing.ListCommentsPros ?? new List<string>(),
                //comment_cons = drawing.CommentCons ?? "",
                list_comments_cons = drawing.ListCommentsCons ?? new List<string>(),
                //comment_style = drawing.ListCommentsStyle ?? new List<string>(),
                list_comments_style = drawing.ListCommentsStyle ?? new List<string>(),
                views = drawing.Views,
                likes = drawing.Likes,
                filter = drawing.Filter,
                model_name = drawing.ModelName ?? "",
                favorite = drawing.Favorite,
                reference_url = drawing.ReferenceUrl ?? "",
                twitter_url = drawing.TwitterUrl ?? "",
                instagram_url = drawing.InstagramUrl ?? "",
                path_thumbnail = drawing.PathThumbnail,
                software = drawing.Software,
                paper = drawing.Paper,
                spotify_url = drawing.SpotifyUrl,
                tags = drawing.Tags,
                votes_popular = drawing.VotesPopular,
                score_critic = drawing.ScoreCritic,
                score_popular = drawing.ScorePopular,
                visible = drawing.Visible
            };
        }
    }
}
