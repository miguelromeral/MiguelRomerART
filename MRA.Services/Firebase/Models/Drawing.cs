using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Models
{
    public class Drawing
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string UrlBase { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

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
                return date.ToString("D", cultureInfo);
            }
        }
    }
}
