using MRA.DTO.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MRA.DTO.ViewModels.Art
{
    public class FilterResults
    {
        [JsonIgnore]
        public List<Drawing> FilteredDrawings { get; set; }
        [JsonIgnore]
        public List<Drawing> TotalDrawings { get; set; }
        public int FetchedCount { get { return (FilteredDrawings != null ? FilteredDrawings.Count : 0); } }
        public int TotalCount { get { return TotalDrawings.Count; } }
        public int TotalTime { get { return TotalDrawings.Sum(x => x.Time); } }
        public bool MoreToFetch { get { return FetchedCount < TotalCount; } }

        public List<string> FilteredDrawingCharacters { get; set; }
        public int NDrawingCharacters { get { return FilteredDrawingCharacters.Count; } }
        public List<string> FilteredDrawingModels { get; set; }
        public int NDrawingModels { get { return FilteredDrawingModels.Count; } }
        public List<int> FilteredDrawingStyles { get; set; }
        public int NDrawingTypes { get { return FilteredDrawingStyles.Count; } }
        public List<int> FilteredDrawingProductTypes { get; set; }
        public int NDrawingProductTypes { get { return FilteredDrawingProductTypes.Count; } }
        public List<string> FilteredDrawingProducts { get; set; }
        public int NDrawingProducts { get { return FilteredDrawingProducts.Count; } }
        public List<int> FilteredDrawingSoftwares { get; set; }
        public int NDrawingSoftwares { get { return FilteredDrawingSoftwares.Count; } }
        public List<int> FilteredDrawingPapers { get; set; }
        public int NDrawingPapers { get { return FilteredDrawingPapers.Count; } }
        public int NDrawingFavorites { get; set; }
        public List<string> FilteredCollections { get; set; }
        public int NDrawingCollections { get { return FilteredCollections.Count; } }

        public FilterResults()
        {
            FilteredDrawings = new List<Drawing>();
            TotalDrawings = new List<Drawing>();
        }

        public FilterResults(List<Drawing> totalDrawings)
        {
            TotalDrawings = totalDrawings;
            FilteredDrawingCharacters = totalDrawings.Select(x => x.Name).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
            FilteredDrawingModels = totalDrawings.Select(x => x.ModelName).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
            FilteredDrawingStyles = totalDrawings.Select(x => x.Type).Distinct().ToList();
            FilteredDrawingProductTypes = totalDrawings.Select(x => x.ProductType).Distinct().ToList();
            FilteredDrawingProducts = totalDrawings.Select(x => x.ProductName).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
            FilteredDrawingSoftwares = totalDrawings.Select(x => x.Software).Distinct().Where(x => x > 0).ToList();
            FilteredDrawingPapers = totalDrawings.Select(x => x.Paper).Distinct().Where(x => x > 0).ToList();
            NDrawingFavorites = totalDrawings.Count(x => x.Favorite);
            FilteredCollections = new List<string>();
            FilteredDrawings = new List<Drawing>();
        }

        public virtual void UpdatefilteredDrawings(List<Drawing> drawings)
        {
            FilteredDrawings = drawings;
        }
    }
}
