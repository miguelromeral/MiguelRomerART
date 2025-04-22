using MRA.DTO.Enums;
using MRA.DTO.Enums.Drawing;
using MRA.DTO.Enums.DrawingFilter;
using MRA.Infrastructure.Enums;
using System.Text.Json.Serialization;

namespace MRA.DTO.ViewModels.Art;

public class DrawingFilter
{
    public const string PRODUCT_NONE = "none";
    public const string CHARACTER_NONE = "none";
    public const string MODEL_NONE = "none";

    [JsonConverter(typeof(EnumStringJsonConverter<DrawingTypes>))]
    public DrawingTypes Type { get; set; }

    public DrawingProductTypes ProductType { get; set; }

    public string ProductName { get; set; }

    public string ModelName { get; set; }

    public string CharacterName { get; set; }

    public string Collection { get; set; }

    public DrawingSoftwares Software { get; set; }

    public DrawingPaperSizes Paper { get; set; }

    [JsonConverter(typeof(EnumStringJsonConverter<DrawingFilterSortBy>))]
    public DrawingFilterSortBy Sortby { get; set; }

    public bool? Spotify { get; set; }

    public string TextQuery { get; set; }
    public IEnumerable<string> Tags { get { return TextQuery.Split(" ").Select(x => x.ToLower()); } }
    
    public bool Favorites { get; set; }
    
    public int PageSize { get; set; }
    
    public int PageNumber { get; set; }
    
    public bool OnlyVisible { get; set; }

    public string CacheKey { get => $"filter_{Type}_{ProductType}_{ProductName}_{ModelName}_{CharacterName}_{Collection}_{Software}_{Paper}_{Sortby}_{Spotify}_{string.Join("_", Tags)}_{Favorites}_{PageSize}_{PageNumber}_{OnlyVisible}"; }


    public static DrawingFilter GetModelNoFilters() =>
        new DrawingFilter()
        {
            Sortby = EnumExtensions.GetDefaultValue<DrawingFilterSortBy>(),
            TextQuery = "",
            Type = EnumExtensions.GetDefaultValue<DrawingTypes>(),
            ProductType = EnumExtensions.GetDefaultValue<DrawingProductTypes>(),
            ProductName = "",
            Collection = "",
            CharacterName = "",
            ModelName = "",
            Software = EnumExtensions.GetDefaultValue<DrawingSoftwares>(),
            Paper = EnumExtensions.GetDefaultValue<DrawingPaperSizes>(),
            Favorites = false,
            Spotify = null,
            OnlyVisible = false,
        };

    public bool OnlyFilterCollection()
    {
        var noFilters = GetModelNoFilters();

        return 
            Sortby == noFilters.Sortby && 
            TextQuery == noFilters.TextQuery && 
            Type == noFilters.Type && 
            ProductType == noFilters.ProductType && 
            ProductName == noFilters.ProductName && 
            Collection != noFilters.Collection && 
            CharacterName == noFilters.CharacterName && 
            ModelName == noFilters.ModelName && 
            Software == noFilters.Software && 
            Paper == noFilters.Paper;
    }
}
