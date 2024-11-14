using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MRA.DTO.Firebase.Models
{
    public class Collection
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public int Order { get; set; }
        
        [JsonIgnore]
        public List<Drawing> Drawings { get; set; }
        
        [JsonIgnore]
        public List<DocumentReference> DrawingsReferences { get; set; }


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
