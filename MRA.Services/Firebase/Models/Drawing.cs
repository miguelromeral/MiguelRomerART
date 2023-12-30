using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MRA.Services.Firebase.Models
{
    public class Drawing
    {
        public static string SEPARATOR_COMMENTS = "#";

        public static Dictionary<int, string> DRAWING_TYPES = new Dictionary<int, string>()
            {
                {1, "Tradicional"},
                {2, "Digital"},
                {3, "Sketch"},
                {4, "Marcadores"},
            };

        public static Dictionary<int, string> DRAWING_PRODUCT_TYPES = new Dictionary<int, string>()
            {
                {0, "Otros"},
                {1, "Videojuego"},
                {2, "Actor/Actriz"},
                {3, "Cantante"},
            };

        public static Dictionary<int, string> DRAWING_SOFTWARE = new Dictionary<int, string>()
            {
                {0, "Ninguno"},
                {1, "Medibang Paint"},
                {2, "Clip Studio Paint"},
                {3, "Adobe Photoshop"},
            };

        public static Dictionary<int, string> DRAWING_PAPER_SIZE = new Dictionary<int, string>()
            {
                {0, "Desconocido"},
                {4, "A4"},
                {5, "A5"},
                {6, "A6"},
            };

        public string Id { get; set; }
        public string Path { get; set; }
        public string PathThumbnail { get; set; }
        public string UrlBase { get; set; }
        public int Type { get; set; }
        public string TypeName { get { return DRAWING_TYPES[Type]; } }
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public int Software { get; set; }
        public string SoftwareName
        {
            get
            {
                try
                {
                    return DRAWING_SOFTWARE[Software];
                }
                catch (Exception ex)
                {
                    return "Ninguno";
                }
            }
        }
        public int Paper { get; set; }
        public string PaperHuman
        {
            get
            {
                try
                {
                    return DRAWING_PAPER_SIZE[Paper];
                }
                catch (Exception ex)
                {
                    return "Otro";
                }
            }
        }
        public int Time { get; set; }
        public string TimeHuman { get
            {
                if (Time > 0)
                {
                    int horas = Time / 60;
                    int minutosRestantes = Time % 60;

                    string resultado = "~ "+(horas > 0 ? horas+"h " : "")+(minutosRestantes > 0 ? minutosRestantes+"min" : "");

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
                try
                {
                    return DRAWING_PRODUCT_TYPES[ProductType];
                }catch(Exception ex)
                {
                    return "Otros";
                }
            } }
        public string ProductName { get; set; }
        public string Comment { get; set; }
        public string CommentPros { get; set; }
        public long Views { get; set; }
        public long Likes { get; set; }
        public bool Favorite { get; set; }
        public string ReferenceUrl { get; set; }

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
        public bool IsTraditional { get { return Type == 1; } }

        public string FormattedDate
        {
            get
            {
                if (String.IsNullOrEmpty(Date))
                    return "";

                DateTime date = DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                //CultureInfo currentCulture = CultureInfo.CurrentCulture;
                var cultureInfo = CultureInfo.GetCultureInfo("es-ES");
                return date.ToString("dd MMMM yyyy", cultureInfo);
            }
        }
    }
}
