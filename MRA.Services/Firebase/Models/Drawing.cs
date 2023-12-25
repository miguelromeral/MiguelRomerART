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

        public string Id { get; set; }
        public string Path { get; set; }
        public string UrlBase { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public string Comment { get; set; }
        public string CommentPros { get; set; }

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
        public bool IsTraditional { get { return Type.Equals("traditional"); } }

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
