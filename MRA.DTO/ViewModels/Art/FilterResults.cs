using MRA.DTO.Models;
using System.Text.Json.Serialization;

namespace MRA.DTO.ViewModels.Art
{
    public class FilterResults
    {
        [JsonIgnore]
        public IEnumerable<DrawingModel> FilteredDrawings { get; set; }
        [JsonIgnore]
        public IEnumerable<DrawingModel> TotalDrawings { get; set; }
        public int FetchedCount { get { return (FilteredDrawings != null ? FilteredDrawings.Count() : 0); } }
        public int TotalCount { get { return TotalDrawings.Count(); } }
        public int TotalTime { get { return TotalDrawings.Sum(x => x.Time); } }
        public bool MoreToFetch { get { return FetchedCount < TotalCount; } }

        public IEnumerable<string> FilteredDrawingCharacters { get; set; }
        public int NDrawingCharacters { get { return FilteredDrawingCharacters.Count(); } }
        public IEnumerable<string> FilteredDrawingModels { get; set; }
        public int NDrawingModels { get { return FilteredDrawingModels.Count(); } }
        public IEnumerable<int> FilteredDrawingStyles { get; set; }
        public int NDrawingTypes { get { return FilteredDrawingStyles.Count(); } }
        public IEnumerable<int> FilteredDrawingProductTypes { get; set; }
        public int NDrawingProductTypes { get { return FilteredDrawingProductTypes.Count(); } }
        public IEnumerable<string> FilteredDrawingProducts { get; set; }
        public int NDrawingProducts { get { return FilteredDrawingProducts.Count(); } }
        public IEnumerable<int> FilteredDrawingSoftwares { get; set; }
        public int NDrawingSoftwares { get { return FilteredDrawingSoftwares.Count(); } }
        public IEnumerable<int> FilteredDrawingPapers { get; set; }
        public int NDrawingPapers { get { return FilteredDrawingPapers.Count(); } }
        public int NDrawingFavorites { get; set; }
        public IEnumerable<string> FilteredCollections { get; set; }
        public int NDrawingCollections { get { return FilteredCollections.Count(); } }

        public FilterResults()
        {

        }

        public FilterResults(IEnumerable<DrawingModel> totalDrawings)
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
            FilteredDrawings = new List<DrawingModel>();
        }

        public virtual void UpdatefilteredDrawings(IEnumerable<DrawingModel> drawings)
        {
            FilteredDrawings = drawings;
        }
    }
}
