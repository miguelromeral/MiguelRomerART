using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using MRA.DTO.Models;
using System.Text.Json.Serialization;

namespace MRA.DTO.ViewModels.Art
{
    public class FilterResults
    {
        [JsonIgnore]
        public IEnumerable<DrawingModel> TotalDrawings { get; set; }
        public IEnumerable<DrawingModel> FilteredDrawings { get; set; }
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


        public FilterResults(IEnumerable<DrawingModel> drawings, IEnumerable<CollectionModel> collections, DrawingFilter filter)
        {
            TotalDrawings = drawings;
            FilteredDrawingCharacters = drawings.Select(x => x.Name).Distinct().Where(x => !string.IsNullOrEmpty(x));
            FilteredDrawingModels = drawings.Select(x => x.ModelName).Distinct().Where(x => !string.IsNullOrEmpty(x));
            FilteredDrawingStyles = drawings.Select(x => (int) x.Type).Distinct();
            FilteredDrawingProductTypes = drawings.Select(x => (int) x.ProductType).Distinct();
            FilteredDrawingProducts = drawings.Select(x => x.ProductName).Distinct().Where(x => !string.IsNullOrEmpty(x));
            FilteredDrawingSoftwares = drawings.Select(x => (int) x.Software).Distinct().Where(x => x > 0);
            FilteredDrawingPapers = drawings.Select(x => (int) x.Paper).Distinct().Where(x => x > 0);
            NDrawingFavorites = drawings.Count(x => x.Favorite);

            var ids = drawings.Select(x => x.Id).ToList();
            FilteredCollections = collections
                .Where(c => c.Drawings.Any(d => ids.Contains(d.Id)))
                .Select(x => x.Id);

            FilteredDrawings = drawings;
            if (filter.PageSize > 0 && filter.PageNumber > 0)
            {
                FilteredDrawings = drawings.Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);
            }
        }
    }
}
