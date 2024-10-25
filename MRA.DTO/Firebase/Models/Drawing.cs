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
        public static string SEPARATOR_COMMENTS = "#";
        public static string SEPARATOR_TAGS = " ";

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

        [ExcelColumn("ID", 1)]
        public string Id { get; set; }
        
        [ExcelColumn("Path", 10)]
        public string Path { get; set; }

        [ExcelColumn("Path Thumbnail", 10)]
        public string PathThumbnail { get; set; }
        public string UrlBase { get; set; }

        [ExcelColumn("Visible", 20)]
        public bool Visible { get; set; }

        [ExcelColumn("#Type", 10)]
        public int Type { get; set; }

        [ExcelColumn("Tags", 10)]
        public string TagsText { get; set; }
        public List<string> Tags { get; set; }
        
        [ExcelColumn("Type", 10)]
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

        [ExcelColumn("Name", 2)]
        public string Name { get; set; }
        
        [ExcelColumn("Model Name", 3)]
        public string ModelName { get; set; }

        [ExcelColumn("Twitter URL", 10)]
        public string TwitterUrl { get; set; }

        [ExcelColumn("Instagram URL", 10)]
        public string InstagramUrl { get; set; }
        
        [ExcelColumn("Spotify URL", 10)]
        public string SpotifyUrl { get; set; }

        [ExcelColumn("Spotify Track ID", 10)]
        public string SpotifyTrackId { 
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

        [ExcelColumn("Title", 10)]
        public string Title { get; set; }

        [ExcelColumn("Date", 10)]
        public string Date { get; set; }

        [ExcelColumn("Date Object", 10)]
        public DateTime DateObject { get; set; }

        [ExcelColumn("Date Hyphen", 10)]
        public string DateHyphen { get; set; }
        
        [ExcelColumn("#Software", 10)]
        public int Software { get; set; }

        [ExcelColumn("Software", 10)]
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

        [ExcelColumn("#Filter", 10)]
        public int Filter { get; set; }

        [ExcelColumn("Filter", 10)]
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

        [ExcelColumn("#Paper", 10)]
        public int Paper { get; set; }

        [ExcelColumn("Paper", 10)]
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

        [ExcelColumn("Time (Minutes)", 10)]
        public int Time { get; set; }

        [ExcelColumn("Time", 10)]
        public string TimeHuman {
            get
            {
                if (Time > 0)
                {
                    int horas = Time / 60;
                    int minutosRestantes = Time % 60;

                    string resultado = (horas > 0 ? horas+"h " : "")+(minutosRestantes > 0 ? minutosRestantes+"min" : "");

                    return resultado;
                }
                else
                {
                    return "Sin Estimación";
                }
            } 
        }

        [ExcelColumn("#Product Type", 10)]
        public int ProductType { get; set; }

        [ExcelColumn("Product Type", 10)]
        public string ProductTypeName {
            get
            {
                    if (DRAWING_PRODUCT_TYPES.ContainsKey(ProductType))
                    {

                    return DRAWING_PRODUCT_TYPES[ProductType];
                    }
                    return "Otros";
            } }

        [ExcelColumn("Product", 10)]
        public string ProductName { get; set; }

        [ExcelColumn("Comments", 10)]
        public string Comment { get; set; }

        [ExcelColumn("Positive Comments", 10)]
        public string CommentPros { get; set; }
        public List<string> ListCommentsStyle { get; set; }

        [ExcelColumn("#Views", 10)]
        public long Views { get; set; }

        [ExcelColumn("Views", 10)]
        public string ViewsHuman { get { return Drawing.FormatoLegible(Views); } }
        
        [ExcelColumn("#Likes", 10)]
        public long Likes { get; set; }
        
        [ExcelColumn("Likes", 10)]
        public string LikesHuman { get { return Drawing.FormatoLegible(Likes); } }
        
        [ExcelColumn("Favorite", 10)]
        public bool Favorite { get; set; }
        
        [ExcelColumn("Reference URL", 10)]
        public string ReferenceUrl { get; set; }

        [ExcelColumn("Score Critic", 10)]
        public int ScoreCritic { get; set; }
        
        [ExcelColumn("Score Popular", 10)]
        public double ScorePopular { get; set; }
        
        [ExcelColumn("Votes Popular", 10)]
        public int VotesPopular { get; set; }
        
        [ExcelColumn("Score Popular (Readable)", 10)]
        public int ScorePopularHuman { get { return CalculateScorePopular(ScorePopular); } }


        public static int CalculateScorePopular(double score) => (int)Math.Round(score);

        [ExcelColumn("List Comments", 10)]
        public List<string> ListComments
        {
            get
            {
                if (String.IsNullOrEmpty(Comment))
                    return new List<string>();
                else
                    return Comment.Split(SEPARATOR_COMMENTS).ToList();
            }
        }

        public List<string> ListCommentPros
        {
            get
            {
                if (String.IsNullOrEmpty(CommentPros))
                    return new List<string>();
                else
                    return CommentPros.Split(SEPARATOR_COMMENTS).ToList();
            }
        }

        [ExcelColumn("Negative Comments", 10)]
        public string CommentCons { get; set; }

        public List<string> ListCommentCons
        {
            get
            {
                if (String.IsNullOrEmpty(CommentCons))
                    return new List<string>();
                else
                    return CommentCons.Split(SEPARATOR_COMMENTS).ToList();
            }
        }

        [ExcelColumn("URL", 10)]
        public string Url { get { return UrlBase + Path; } }
        
        [ExcelColumn("URL Thumbnail", 10)]
        public string UrlThumbnail { get { return UrlBase + PathThumbnail; } }

        
        [ExcelColumn("Popularity Date", 10)]
        public double PopularityDate { get; set; }

        [ExcelColumn("Popularity Critic", 10)]
        public double PopularityCritic { get; set; }
        
        [ExcelColumn("Popularity Popular", 10)]
        public double PopularityPopular { get; set; }

        [ExcelColumn("Popularity Favorite", 10)]
        public double PopularityFavorite{ get; set; }

        [ExcelColumn("Popularity", 10)]
        public double Popularity { get {
                return PopularityDate + PopularityCritic + PopularityPopular + PopularityFavorite;
            }
        }

        public double CalculatePopularity(double dateWeight, int months, double criticWeight, double popularWeight, double favoriteWeight)
        {
            PopularityCritic = Utilities.CalculatePopularity(DateObject, dateWeight, DateTime.Now.AddMonths(- months), DateTime.Now);
            PopularityDate = Utilities.CalculatePopularity(ScoreCritic, criticWeight);
            PopularityPopular = Utilities.CalculatePopularity(ScorePopular, popularWeight);
            PopularityFavorite = (Favorite ? favoriteWeight : 0);

            //Debug.WriteLine($"{dateWeight.ToString().PadRight(5)} {months.ToString().PadRight(5)} {criticWeight.ToString().PadRight(5)} " +
                //$"{popularWeight.ToString().PadRight(5)} {favoriteWeight.ToString().PadRight(5)} ");
            return Popularity;
        }

        [ExcelColumn("Formatted Date", 10)]
        public string FormattedDate
        {
            get
            {
                return Utilities.FormattedDate(Date);
            }
        }

        [ExcelColumn("Formatted Date (Mini)", 10)]
        public string FormattedDateMini
        {
            get
            {
                return Utilities.FormattedDateMini(Date);
            }
        }

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
