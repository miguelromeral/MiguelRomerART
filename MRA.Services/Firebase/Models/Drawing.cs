using System;
using System.Collections.Generic;
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

        public string Url { get { return UrlBase + Path; } }
        public bool IsTraditional { get { return Type.Equals("traditional"); } }
    }
}
