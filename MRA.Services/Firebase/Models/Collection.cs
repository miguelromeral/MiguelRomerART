using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Models
{
    public class Collection
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public int Order { get; set; }
        public List<Drawing> Drawings { get; set; }
        public List<DocumentReference> DrawingsReferences { get; set; }

        public string TextDrawingsReferences
        {
            get
            {
                return Environment.NewLine + String.Join(", "+ Environment.NewLine, DrawingsReferences.Select(x => "* "+x.Parent + "/" + x.Id).ToList());
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

            return sb.ToString();
        }
    }
}
