using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Models
{
    public class FilterResults
    {
        public List<Drawing> FilteredDrawings { get; private set; }
        public int FetchedCount { get { return FilteredDrawings.Count; } }
        public int TotalCount { get; private set; }
        public int TotalTime { get; private set; }
        public bool MoreToFetch { get { return FetchedCount < TotalCount; } }

        public List<string> FilteredDrawingCharacters { get; private set; }
        public int NDrawingCharacters { get { return FilteredDrawingCharacters.Count; } }
        public List<string> FilteredDrawingModels { get; private set; }
        public int NDrawingModels { get { return FilteredDrawingModels.Count; } }
        public List<int> FilteredDrawingStyles { get; private set; }
        public int NDrawingTypes { get { return FilteredDrawingStyles.Count; } }
        public List<int> FilteredDrawingProductTypes { get; private set; }
        public int NDrawingProductTypes { get { return FilteredDrawingProductTypes.Count; } }
        public List<string> FilteredDrawingProducts { get; private set; }
        public int NDrawingProducts { get { return FilteredDrawingProducts.Count; } }
        public List<int> FilteredDrawingSoftwares { get; private set; }
        public int NDrawingSoftwares { get { return FilteredDrawingSoftwares.Count; } }
        public List<int> FilteredDrawingPapers { get; private set; }
        public int NDrawingPapers { get { return FilteredDrawingPapers.Count; } }
        public int NDrawingFavorites { get; private set; }
        public List<string> FilteredCollections { get; set; }
        public int NDrawingCollections { get { return FilteredCollections.Count; } }

        public FilterResults(List<Drawing> totalDrawings) 
        {
            TotalCount = totalDrawings.Count;
            TotalTime = totalDrawings.Sum(x => x.Time);
            FilteredDrawingCharacters = totalDrawings.Select(x => x.Name).Distinct().Where(x => !String.IsNullOrEmpty(x)).ToList();
            FilteredDrawingModels = totalDrawings.Select(x => x.ModelName).Distinct().Where(x => !String.IsNullOrEmpty(x)).ToList();
            FilteredDrawingStyles = totalDrawings.Select(x => x.Type).Distinct().ToList();
            FilteredDrawingProductTypes = totalDrawings.Select(x => x.ProductType).Distinct().ToList();
            FilteredDrawingProducts = totalDrawings.Select(x => x.ProductName).Distinct().Where(x => !String.IsNullOrEmpty(x)).ToList();
            FilteredDrawingSoftwares = totalDrawings.Select(x => x.Software).Distinct().Where(x => x > 0).ToList();
            FilteredDrawingPapers = totalDrawings.Select(x => x.Paper).Distinct().Where(x => x > 0).ToList();
            NDrawingFavorites = totalDrawings.Count(x => x.Favorite);
        }

        public void UpdatefilteredDrawings(List<Drawing> drawings)
        {
            FilteredDrawings = drawings;
        }
    }
}
