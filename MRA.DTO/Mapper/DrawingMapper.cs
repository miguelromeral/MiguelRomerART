using MRA.Infrastructure.Settings;
using MRA.DTO.Models;
using MRA.DTO.Mapper.Interfaces;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Documents.MongoDb;

namespace MRA.DTO.Mapper;

public class DrawingMapper : IDocumentMapper<DrawingModel, IDrawingDocument>
{
    private readonly string _urlBase;

    public DrawingMapper(AppSettings appConfig)
    {
        _urlBase = appConfig.AzureStorage.BlobPath;
    }

    public DrawingModel ConvertToModel(IDrawingDocument drawingDocument)
    {
        return new DrawingModel
        {
            Id = drawingDocument.Id,
            Path = drawingDocument.path,
            Type = drawingDocument.type,
            Title = drawingDocument.title,
            Name = drawingDocument.name,
            Date = drawingDocument.drawingAt.ToString("yyyy/MM/dd"),
            DateHyphen = drawingDocument.drawingAt.ToString("yyyy-MM-dd"),
            DateObject = drawingDocument.drawingAt,
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

    public IDrawingDocument ConvertToDocument(DrawingModel drawing)
    {
        return new DrawingMongoDocument
        {
            Id = drawing.Id,
            path = drawing.Path ?? "",
            type = drawing.Type,
            title = drawing.Title ?? "",
            drawingAt = drawing.DateObject.ToUniversalTime(),
            time = drawing.Time,
            name = drawing.Name ?? "",
            product_type = drawing.ProductType,
            product_name = drawing.ProductName ?? "",
            list_comments = drawing.ListComments ?? new List<string>(),
            list_comments_pros = drawing.ListCommentsPros ?? new List<string>(),
            list_comments_cons = drawing.ListCommentsCons ?? new List<string>(),
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
