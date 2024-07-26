using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.ViewModels.Art
{
    public class DrawingFilter
    {
        public int Type { get; set; }
        public int ProductType { get; set; }
        public string? ProductName { get; set; }
        public string? ModelName { get; set; }
        public string? CharacterName { get; set; }
        public string? Collection { get; set; }
        public int Software { get; set; }
        public int Paper { get; set; }
        public string? Sortby { get; set; }
        public bool? Spotify { get; set; }
        public string? TextQuery { get; set; }
        public List<string> Tags { get { return (TextQuery ?? "").Split(" ").Select(x => x.ToLower()).ToList(); } }
        public bool Favorites { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public string CacheKey { get => $"filter_{Type}_{ProductType}_{ProductName}_{ModelName}_{CharacterName}_{Collection}_{Software}_{Paper}_{Sortby}_{Spotify}_{string.Join("_", Tags)}_{Favorites}_{PageSize}_{PageNumber}"; }


        public static DrawingFilter GetModelNoFilters() =>
            new DrawingFilter()
            {
                Sortby = "date-desc",
                TextQuery = "",
                Type = -1,
                ProductType = -1,
                ProductName = "",
                Collection = "",
                CharacterName = "",
                ModelName = "",
                Software = 0,
                Paper = 0,
                Favorites = false,
                Spotify = null,
            };

        public bool HasNoFilters()
        {
            var nofilters = DrawingFilter.GetModelNoFilters();
            return
                (Sortby ?? "").Equals(nofilters.Sortby) &&
                (TextQuery ?? "").Equals(nofilters.TextQuery) &&
                Type.Equals(nofilters.Type) &&
                ProductType.Equals(nofilters.ProductType) &&
                (ProductName ?? "").Equals(nofilters.ProductName) &&
                (Collection ?? "").Equals(nofilters.Collection) &&
                (CharacterName ?? "").Equals(nofilters.CharacterName) &&
                (ModelName ?? "").Equals(nofilters.ModelName) &&
                Software.Equals(nofilters.Software) &&
                Paper.Equals(nofilters.Paper) &&
                Spotify == nofilters.Spotify &&
                Favorites.Equals(nofilters.Favorites);
        }
    }
}
