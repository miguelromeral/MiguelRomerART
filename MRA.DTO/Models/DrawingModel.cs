using MRA.DTO.Enums.Drawing;
using MRA.DTO.Models.Interfaces;
using MRA.Extensions;
using MRA.Infrastructure.Enums;
using MRA.Infrastructure.Excel.Attributes;
using System.Text;

namespace MRA.DTO.Models;

public class DrawingModel : IModel
{
    public const string SEPARATOR_COMMENTS = "#";

    public DrawingModel()
    {   
        Tags = new List<string>();
        ListComments = new List<string>();
        ListCommentsStyle = new List<string>();
        ListCommentsPros = new List<string>();
        ListCommentsCons = new List<string>();
    }


    #region Document Data
    [ExcelColumn("ID", 1, width: 30)]
    public string Id { get; set; }
    #endregion

    #region Azure Image
    public string UrlBase { get; set; }

    [ExcelColumn("Path", 10)]
    public string Path { get; set; }

    [ExcelColumn("URL", 11, url: true, wrapText: true)]
    public string Url { get { return UrlBase + Path; } }

    [ExcelColumn("Path Thumbnail", 15)]
    public string PathThumbnail { get; set; }

    [ExcelColumn("URL Thumbnail", 16, url: true, wrapText: true)]
    public string UrlThumbnail { get { return UrlBase + PathThumbnail; } }
    #endregion

    #region Title
    [ExcelColumn("Title", 20)]
    public string Title { get; set; }

    [ExcelColumn("Favorite", 21, width: 10)]
    public bool Favorite { get; set; }
    #endregion

    #region Character

    [ExcelColumn("Name", 30, width: 20)]
    public string Name { get; set; }

    [ExcelColumn("Model Name", 31, width: 20)]
    public string ModelName { get; set; }
    #endregion

    #region Product
    [ExcelColumn("#Product Type", 40, width: 5)]
    public DrawingProductTypes ProductType { get; set; }

    [ExcelColumn("Product Type", 41, width: 15)]
    public string ProductTypeName { get => ProductType.GetDescription(); }
    

    [ExcelColumn("Product", 42, width: 30)]
    public string ProductName { get; set; }
    #endregion

    #region Style
    [ExcelColumn("#Type", 50, width: 5)]
    public DrawingTypes Type { get; set; }

    [ExcelColumn("Type", 51, width: 20)]
    public string TypeName { get => Type.GetDescription(); }

    [ExcelColumn("#Software", 52, width: 5)]
    public DrawingSoftwares Software { get; set; }

    [ExcelColumn("Software", 53, width: 20)]
    public string SoftwareName { get => Software.GetDescription(); }

    [ExcelColumn("#Paper", 54, width: 5)]
    public DrawingPaperSizes Paper { get; set; }

    [ExcelColumn("Paper", 55, width: 10)]
    public string PaperHuman { get => Paper.GetDescription(); }

    [ExcelColumn("#Filter", 56, width: 5)]
    public DrawingFilterTypes Filter { get; set; }

    [ExcelColumn("Filter", 57, width: 20)]
    public string FilterName { get => Filter.GetDescription(); }
    #endregion

    #region Details
    public string Date { get; set; }

    [ExcelColumn("Date", 60, width: 15)]
    public DateTime DateObject { get; set; }

    public string DateHyphen { get; set; }

    [ExcelColumn("Formatted Date", 63, width: 20)]
    public string FormattedDate { get => DateObject.FormattedDate(); }

    [ExcelColumn("Time (Minutes)", 65, width: 8)]
    public int Time { get; set; }

    [ExcelColumn("Time", 66, width: 12)]
    public string TimeHuman { get => Time.GetHumanTime(); }

    [ExcelColumn("Views", 67, width: 8)]
    public long Views { get; set; }

    public string ViewsHuman => Views.HumanFormat();

    [ExcelColumn("Likes", 69, width: 8)]
    public long Likes { get; set; }

    public string LikesHuman => Likes.HumanFormat();
    #endregion

    #region Scores
    [ExcelColumn("Score Critic", 70, width: 10)]
    public int ScoreCritic { get; set; }

    [ExcelColumn("Score Popular", 71, width: 10)]
    public double ScorePopular { get; set; }

    [ExcelColumn("Votes Popular", 72, width: 10)]
    public int VotesPopular { get; set; }

    public int ScorePopularHuman { get => CalculateScorePopular(ScorePopular); }
    public static int CalculateScorePopular(double score) => (int)Math.Round(score);
    #endregion

    #region Comments
    [ExcelColumn("List Comments", 81)]
    public IEnumerable<string> ListComments { get; set; }
    #endregion

    #region Style Comments
    [ExcelColumn("Style Comment", 91)]
    public IEnumerable<string> ListCommentsStyle { get; set; }
    #endregion

    #region Positive Comments

    [ExcelColumn("Positive Comments", 100)]
    public IEnumerable<string> ListCommentsPros { get; set; }
    #endregion

    #region Negative Comments

    [ExcelColumn("Negative Comments", 100)]
    public IEnumerable<string> ListCommentsCons { get; set; }
    #endregion

    #region References
    [ExcelColumn("Reference URL", 120, url: true, wrapText: true)]
    public string ReferenceUrl { get; set; }

    [ExcelColumn("Spotify URL", 121, url: true, wrapText: true)]
    public string SpotifyUrl { get; set; }

    [ExcelColumn("Spotify Track ID", 122, width: 30)]
    public string SpotifyTrackId { get => SpotifyUrl.GetSpotifyTrackId(); }

    [ExcelColumn("Visible", 123, width: 10)]
    public bool Visible { get; set; }
    #endregion

    #region Social Networks
    [ExcelColumn("Twitter URL", 130, url: true, wrapText: true)]
    public string TwitterUrl { get; set; }

    [ExcelColumn("Instagram URL", 131, url: true, wrapText: true)]
    public string InstagramUrl { get; set; }
    #endregion

    #region Tags
    public string TagsText { get; set; }

    [ExcelColumn("Tags", 140)]
    public IEnumerable<string> Tags { get; set; }
    #endregion

    #region Popularity
    [ExcelColumn("Popularity", 150, ignoreOnImport: true, width: 10)]
    public double Popularity
    {
        get
        {
            return PopularityDate + PopularityCritic + PopularityPopular + PopularityFavorite;
        }
    }

    [ExcelColumn("Popularity Date", 151, ignoreOnImport: true, width: 20)]
    public double PopularityDate { get; set; }

    [ExcelColumn("Popularity Critic", 152, ignoreOnImport: true, width: 20)]
    public double PopularityCritic { get; set; }

    [ExcelColumn("Popularity Popular", 153, ignoreOnImport: true, width: 20)]
    public double PopularityPopular { get; set; }

    [ExcelColumn("Popularity Favorite", 154, ignoreOnImport: true, width: 20)]
    public double PopularityFavorite { get; set; }

    public double CalculatePopularity(double dateWeight, int months, double criticWeight, double popularWeight, double favoriteWeight)
    {
        PopularityCritic = Utilities.CalculatePopularity(DateObject, dateWeight, DateTime.Now.AddMonths(-months), DateTime.Now);
        PopularityDate = Utilities.CalculatePopularity(ScoreCritic, criticWeight);
        PopularityPopular = Utilities.CalculatePopularity(ScorePopular, popularWeight);
        PopularityFavorite = (Favorite ? favoriteWeight : 0);
        return Popularity;
    }
    #endregion

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(Id);

        if (!String.IsNullOrEmpty(Name))
        {
            sb.Append($" ({Name})");
        }
        if (!String.IsNullOrEmpty(ModelName))
        {
            sb.Append($" [{ModelName}]");
        }

        return sb.ToString();
    }

    public string GetId() => Id;
}
