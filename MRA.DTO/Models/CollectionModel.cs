using Google.Cloud.Firestore;
using MRA.DTO.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MRA.DTO.Models;

public class CollectionModel : IModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    
    [JsonIgnore]
    public IEnumerable<DrawingModel> Drawings { get; set; }

    public IEnumerable<string> DrawingIds { get; set; }

    public CollectionModel()
    {
        Drawings = new List<DrawingModel>();
        DrawingIds = new List<string>();
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

    public string GetId() => Id;
}
