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
                {1, "Traditional"},
                {2, "Digital"},
                {3, "Quick Sketch"},
                {4, "Markers"},
            };

        public static Dictionary<int, string> DRAWING_PRODUCT_TYPES = new Dictionary<int, string>()
            {
                {1, "Videogame"},
                {2, "Actor/Actress"},
            };

        public string Id { get; set; }
        public string Path { get; set; }
        public string UrlBase { get; set; }
        public int Type { get; set; }
        public string TypeName { get { return DRAWING_TYPES[Type]; } }
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
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
                    return "No Time Known";
                }
            } 
        }
        public int ProductType { get; set; }
        public string ProductTypeName { get { return DRAWING_PRODUCT_TYPES[ProductType]; } }
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
        public bool IsTraditional { get { return Type == 1; } }

        public string FormattedDate
        {
            get
            {
                if (String.IsNullOrEmpty(Date))
                    return "";

                DateTime date = DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                //CultureInfo currentCulture = CultureInfo.CurrentCulture;
                var cultureInfo = CultureInfo.GetCultureInfo("en-US");
                return date.ToString("dd MMMM yyyy", cultureInfo);
            }
        }
    }
}
