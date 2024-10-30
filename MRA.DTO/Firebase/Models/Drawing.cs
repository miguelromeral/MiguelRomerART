using Google.Protobuf;
using MRA.DTO;
using MRA.DTO.Excel.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MRA.DTO.Firebase.Models
{
    public class Drawing
    {
        public const string SEPARATOR_COMMENTS = "#";
        public const string SEPARATOR_TAGS = " ";

        public const string EXCEL_FAVORITE_VALUE = "Favorite";
        public const string EXCEL_VISIBLE_VALUE = "Visible";
        public const string EXCEL_SEPARATOR_COMMENTS = "\n";

        public Drawing()
        {
            
            Tags = new List<string>();
        }

        public static Dictionary<int, string> DRAWING_TYPES = new Dictionary<int, string>()
            {
                {0, "Otros"},
                {1, "Lápices de Grafito"},
                {2, "Digital"},
                {3, "Sketch"},
                {4, "Marcadores"},
                {5, "Lápices de Colores"},
                {6, "Bolígrafo"},
                {7, "Line Art"},
            };

        public static Dictionary<int, string> DRAWING_PRODUCT_TYPES = new Dictionary<int, string>()
            {
                {0, "Otros"},
                {1, "Videojuego"},
                {2, "Actor / Actriz"},
                {3, "Cantante"},
                {4, "Deportista"},
                {5, "Influencer"},
            };

        public static Dictionary<int, string> DRAWING_SOFTWARE = new Dictionary<int, string>()
            {
                {0, "Ninguno"},
                {1, "Medibang Paint"},
                {2, "Clip Studio Paint"},
                {3, "Adobe Photoshop"},
                {4, "GIMP"},
            };

        public static Dictionary<int, string> DRAWING_PAPER_SIZE = new Dictionary<int, string>()
            {
                {0, "Desconocido"},
                {1, "A1"},
                {2, "A2"},
                {3, "A3"},
                {4, "A4"},
                {5, "A5"},
                {6, "A6"},
            };

        public static Dictionary<int, string> DRAWING_FILTER = new Dictionary<int, string>()
            {
                {0, "Desconocido"},
                {1, "Snapseed"},
                {2, "Adobe Photoshop"},
                {3, "Instagram"},
                {4, "Samsung Galaxy"},
            };

        #region Document Data
        [ExcelColumn("ID", 1)]
        public string Id { get; set; }
        #endregion

        #region Azure Image
        public string UrlBase { get; set; }

        [ExcelColumn("Path", 10)]
        public string Path { get; set; }

        [ExcelColumn("URL", 11, hidden: true, url: true)]
        public string Url { get { return UrlBase + Path; } }

        [ExcelColumn("Path Thumbnail", 15)]
        public string PathThumbnail { get; set; }

        [ExcelColumn("URL Thumbnail", 16, hidden: true, url: true)]
        public string UrlThumbnail { get { return UrlBase + PathThumbnail; } }
        #endregion

        #region Title
        [ExcelColumn("Title", 20)]
        public string Title { get; set; }

        [ExcelColumn("Favorite", 21)]
        public bool Favorite { get; set; }
        #endregion

        #region Character

        [ExcelColumn("Name", 30)]
        public string Name { get; set; }

        [ExcelColumn("Model Name", 31)]
        public string ModelName { get; set; }
        #endregion

        #region Product
        [ExcelColumn("#Product Type", 40, hidden: true)]
        public int ProductType { get; set; }

        [ExcelColumn("Product Type", 41)]
        public string ProductTypeName
        {
            get
            {
                if (DRAWING_PRODUCT_TYPES.ContainsKey(ProductType))
                {

                    return DRAWING_PRODUCT_TYPES[ProductType];
                }
                return "Otros";
            }
        }

        [ExcelColumn("Product", 42)]
        public string ProductName { get; set; }
        #endregion

        #region Style
        [ExcelColumn("#Type", 50, hidden: true)]
        public int Type { get; set; }

        [ExcelColumn("Type", 51)]
        public string TypeName
        {
            get
            {

                if (DRAWING_TYPES.ContainsKey(Type))
                {

                    return DRAWING_TYPES[Type];
                }
                return "Otros";
            }
        }

        [ExcelColumn("#Software", 52, hidden: true)]
        public int Software { get; set; }

        [ExcelColumn("Software", 53)]
        public string SoftwareName
        {
            get
            {
                if (DRAWING_SOFTWARE.ContainsKey(Software))
                {

                    return DRAWING_SOFTWARE[Software];
                }
                return "Ninguno";
            }
        }

        [ExcelColumn("#Paper", 54, hidden: true)]
        public int Paper { get; set; }

        [ExcelColumn("Paper", 55)]
        public string PaperHuman
        {
            get
            {
                if (DRAWING_PAPER_SIZE.ContainsKey(Paper))
                {

                    return DRAWING_PAPER_SIZE[Paper];
                }
                return "Otro";
            }
        }

        [ExcelColumn("#Filter", 56, hidden: true)]
        public int Filter { get; set; }

        [ExcelColumn("Filter", 57)]
        public string FilterName
        {
            get
            {
                if (DRAWING_FILTER.ContainsKey(Filter))
                {
                    return DRAWING_FILTER[Filter];
                }
                return "Ninguno";
            }
        }
        #endregion

        #region Details
        public string Date { get; set; }

        [ExcelColumn("Date", 60)]
        public DateTime DateObject { get; set; }

        public string DateHyphen { get; set; }

        [ExcelColumn("Formatted Date", 63)]
        public string FormattedDate
        {
            get
            {
                return Utilities.FormattedDate(Date);
            }
        }

        public string FormattedDateMini
        {
            get
            {
                return Utilities.FormattedDateMini(Date);
            }
        }

        [ExcelColumn("Time (Minutes)", 65)]
        public int Time { get; set; }

        [ExcelColumn("Time", 66)]
        public string TimeHuman
        {
            get
            {
                if (Time > 0)
                {
                    int horas = Time / 60;
                    int minutosRestantes = Time % 60;

                    string resultado = (horas > 0 ? horas + "h " : "") + (minutosRestantes > 0 ? minutosRestantes + "min" : "");

                    return resultado;
                }
                else
                {
                    return "Sin Estimación";
                }
            }
        }

        [ExcelColumn("Views", 67)]
        public long Views { get; set; }

        public string ViewsHuman { get { return Drawing.FormatoLegible(Views); } }

        [ExcelColumn("Likes", 69)]
        public long Likes { get; set; }

        public string LikesHuman { get { return Drawing.FormatoLegible(Likes); } }
        #endregion

        #region Scores
        [ExcelColumn("Score Critic", 70)]
        public int ScoreCritic { get; set; }

        [ExcelColumn("Score Popular", 71)]
        public double ScorePopular { get; set; }

        [ExcelColumn("Votes Popular", 72)]
        public int VotesPopular { get; set; }

        public int ScorePopularHuman { get { return CalculateScorePopular(ScorePopular); } }
        public static int CalculateScorePopular(double score) => (int)Math.Round(score);
        #endregion

        #region Comments
        //public string Comment { get; set; }

        [ExcelColumn("List Comments", 81)]
        public List<string> ListComments { get; set; }
        #endregion

        #region Style Comments
        [ExcelColumn("Style Comment", 91)]
        public List<string> ListCommentsStyle { get; set; }
        #endregion

        #region Positive Comments
        //public string CommentPros { get; set; }

        [ExcelColumn("Positive Comments", 100)]
        public List<string> ListCommentsPros { get; set; }
        #endregion

        #region Negative Comments
        //public string CommentCons { get; set; }

        [ExcelColumn("Negative Comments", 100)]
        public List<string> ListCommentsCons { get; set; }
        #endregion

        #region References
        [ExcelColumn("Reference URL", 120, hidden: true, url: true)]
        public string ReferenceUrl { get; set; }

        [ExcelColumn("Spotify URL", 121, hidden: true, url: true)]
        public string SpotifyUrl { get; set; }

        [ExcelColumn("Spotify Track ID", 122)]
        public string SpotifyTrackId
        {
            get
            {
                return String.IsNullOrEmpty(SpotifyUrl) ? "" : GetSpotifyTrackByUrl(SpotifyUrl);
            }
        }

        public static string GetSpotifyTrackByUrl(string url)
        {
            string pattern = @"\/track\/([^\/?]+)(?:\?|$)";

            Regex regex = new Regex(pattern);

            Match match = regex.Match(url);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

        [ExcelColumn("Visible", 123)]
        public bool Visible { get; set; }
        #endregion

        #region Social Networks
        [ExcelColumn("Twitter URL", 130, url: true)]
        public string TwitterUrl { get; set; }

        [ExcelColumn("Instagram URL", 131, url: true)]
        public string InstagramUrl { get; set; }
        #endregion

        #region Tags
        public string TagsText { get; set; }

        [ExcelColumn("Tags", 140, hidden: true)]
        public List<string> Tags { get; set; }
        #endregion

        #region Popularity
        [ExcelColumn("Popularity", 150, ignoreOnImport: true)]
        public double Popularity
        {
            get
            {
                return PopularityDate + PopularityCritic + PopularityPopular + PopularityFavorite;
            }
        }

        [ExcelColumn("Popularity Date", 151, hidden: true, ignoreOnImport: true)]
        public double PopularityDate { get; set; }

        [ExcelColumn("Popularity Critic", 152, hidden: true, ignoreOnImport: true)]
        public double PopularityCritic { get; set; }

        [ExcelColumn("Popularity Popular", 153, hidden: true, ignoreOnImport: true)]
        public double PopularityPopular { get; set; }

        [ExcelColumn("Popularity Favorite", 154, hidden: true, ignoreOnImport: true)]
        public double PopularityFavorite { get; set; }

        public double CalculatePopularity(double dateWeight, int months, double criticWeight, double popularWeight, double favoriteWeight)
        {
            PopularityCritic = Utilities.CalculatePopularity(DateObject, dateWeight, DateTime.Now.AddMonths(-months), DateTime.Now);
            PopularityDate = Utilities.CalculatePopularity(ScoreCritic, criticWeight);
            PopularityPopular = Utilities.CalculatePopularity(ScorePopular, popularWeight);
            PopularityFavorite = (Favorite ? favoriteWeight : 0);

            //Debug.WriteLine($"{dateWeight.ToString().PadRight(5)} {months.ToString().PadRight(5)} {criticWeight.ToString().PadRight(5)} " +
            //$"{popularWeight.ToString().PadRight(5)} {favoriteWeight.ToString().PadRight(5)} ");
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

        public static string FormatoLegible(long numero)
        {
            const long UN_MILLON = 1000000;
            const long MIL = 1000;

            if (numero < MIL)
            {
                return numero.ToString();
            }
            else if (numero < UN_MILLON)
            {
                double valorFormateado = Math.Round((double)numero / MIL, 1);
                return $"{valorFormateado} k";
            }
            else
            {
                double valorFormateado = Math.Round((double)numero / UN_MILLON, 1);
                return $"{valorFormateado} M";
            }
        }
    }
}
