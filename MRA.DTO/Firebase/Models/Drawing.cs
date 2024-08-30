using Google.Protobuf;
using MRA.DTO;
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

        public string Id { get; set; }
        public string Path { get; set; }
        public string PathThumbnail { get; set; }
        public string UrlBase { get; set; }
        public bool Visible { get; set; }
        public int Type { get; set; }
        public string TagsText { get; set; }
        public List<string> Tags { get; set; }
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
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string TwitterUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string SpotifyUrl { get; set; }
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

        public string Title { get; set; }
        public string Date { get; set; }
        public DateTime DateObject { get; set; }
        public string DateHyphen { get; set; }
        public int Software { get; set; }
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
        public int Paper { get; set; }
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
        public int Time { get; set; }
        public string TimeHuman { get
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
        public int ProductType { get; set; }
        public string ProductTypeName { get
            {
                    if (DRAWING_PRODUCT_TYPES.ContainsKey(ProductType))
                    {

                    return DRAWING_PRODUCT_TYPES[ProductType];
                    }
                    return "Otros";
            } }
        public string ProductName { get; set; }
        public string Comment { get; set; }
        public string CommentPros { get; set; }
        public long Views { get; set; }
        public string ViewsHuman { get { return Drawing.FormatoLegible(Views); } }
        public long Likes { get; set; }
        public string LikesHuman { get { return Drawing.FormatoLegible(Likes); } }
        public bool Favorite { get; set; }
        public string ReferenceUrl { get; set; }

        public int ScoreCritic { get; set; }
        public double ScorePopular { get; set; }
        public int VotesPopular { get; set; }
        public int ScorePopularHuman { get { return CalculateScorePopular(ScorePopular); } }


        public static int CalculateScorePopular(double score) => (int)Math.Round(score);

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

        public string Url { get { return UrlBase + Path; } }
        public string UrlThumbnail { get { return UrlBase + PathThumbnail; } }

        
        public double PopularityDate { get; set; }
        public double PopularityCritic { get; set; }
        public double PopularityPopular { get; set; }
        public double PopularityFavorite{ get; set; }
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

            Debug.WriteLine($"{dateWeight.ToString().PadRight(5)} {months.ToString().PadRight(5)} {criticWeight.ToString().PadRight(5)} " +
                $"{popularWeight.ToString().PadRight(5)} {favoriteWeight.ToString().PadRight(5)} ");
            return Popularity;
        }

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
