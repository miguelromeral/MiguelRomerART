using MRA.DTO.Enums.Drawing;
using MRA.DTO.Enums.DrawingFilter;
using MRA.DTO.Models;
using MRA.Infrastructure.Enums;
using MRA.Infrastructure.Settings;

namespace MRA.Services.Models.Drawings;

public class DrawingTagManager
{
    public const string TAG_SEPARATOR = " ";

    private readonly AppSettings _appConfig;


    public DrawingTagManager(AppSettings appConfig)
    {
        _appConfig = appConfig;
    }


    public DrawingModel SetAutomaticTags(DrawingModel document)
    {
        var list = new List<string>();
        list = SetAutomaticTags_Name(document, list).ToList();
        list = SetAutomaticTags_ModelName(document, list).ToList();
        list = SetAutomaticTags_Title(document, list).ToList();
        list = SetAutomaticTags_Software(document, list).ToList();
        list = SetAutomaticTags_Paper(document, list).ToList();
        list = SetAutomaticTags_Type(document, list).ToList();
        list = SetAutomaticTags_ProductType(document, list).ToList();
        list = SetAutomaticTags_ProductName(document, list).ToList();
        list = SetAutomaticTags_Tags(document, list).ToList();
        document.Tags = DeleteAndAdjustTags(list);

        return document;
    }

    private static IEnumerable<string> SetAutomaticTags_Name(DrawingModel document, List<string> list)
    {
        list.AddRange(document.Name.Split(TAG_SEPARATOR).Select(x => x.ToLower()));
        return list;
    }

    private static IEnumerable<string> SetAutomaticTags_ModelName(DrawingModel document, List<string> list)
    {
        list.AddRange(document.ModelName.Split(TAG_SEPARATOR).Select(x => x.ToLower()));
        return list;
    }

    private static IEnumerable<string> SetAutomaticTags_Title(DrawingModel document, List<string> list)
    {
        list.AddRange(document.Title.Split(TAG_SEPARATOR).Select(x => x.ToLower()));
        return list;
    }

    private static IEnumerable<string> SetAutomaticTags_Software(DrawingModel document, List<string> list)
    {
        if (document.Software != EnumExtensions.GetDefaultValue<DrawingSoftwares>())
        {
            list.AddRange(document.SoftwareName.Split(TAG_SEPARATOR).Select(x => x.ToLower()));
        }
        return list;
    }

    private static IEnumerable<string> SetAutomaticTags_Paper(DrawingModel document, List<string> list)
    {
        if (document.Paper != EnumExtensions.GetDefaultValue<DrawingPaperSizes>())
        {
            list.AddRange(document.PaperHuman.Split(TAG_SEPARATOR).Select(x => x.ToLower()));
        }
        return list;
    }

    private static IEnumerable<string> SetAutomaticTags_Type(DrawingModel document, List<string> list)
    {
        if (document.Type != EnumExtensions.GetDefaultValue<DrawingTypes>())
        {
            list.AddRange(document.TypeName.Split(TAG_SEPARATOR).Select(x => x.ToLower()));
        }
        return list;
    }

    private static IEnumerable<string> SetAutomaticTags_ProductType(DrawingModel document, List<string> list)
    {
        if (document.ProductType != EnumExtensions.GetDefaultValue<DrawingProductTypes>())
        {
            list.AddRange(document.ProductTypeName.Split(TAG_SEPARATOR).Select(x => x.ToLower()));
        }
        return list;
    }

    private static IEnumerable<string> SetAutomaticTags_ProductName(DrawingModel document, List<string> list)
    {
        list.AddRange(document.ProductName.Split(TAG_SEPARATOR).Select(x => x.ToLower()));
        return list;
    }

    private static IEnumerable<string> SetAutomaticTags_Tags(DrawingModel document, List<string> list)
    {
        list.AddRange(document.Tags);
        return list;
    }

    public IEnumerable<string> DeleteAndAdjustTags(IEnumerable<string> tags)
    {
        var toDelete = _appConfig.Database.Drawings.Tags.Delete;
        var toReplace = _appConfig.Database.Drawings.Tags.Replace;

        var processedTags = tags.Select(tag => ReplaceCharacters(tag, toReplace));

        var filteredTags = processedTags
            .SelectMany(tag => SplitAndFilter(tag, toDelete))
            .Distinct();

        return filteredTags;
    }

    private static string ReplaceCharacters(string tag, IDictionary<string, string> toReplace)
    {
        var result = tag.ToLower();
        foreach (var change in toReplace)
        {
            result = result.Replace(change.Key, change.Value);
        }
        return result;
    }

    private static IEnumerable<string> SplitAndFilter(string tag, IEnumerable<string> toDelete)
    {
        return tag.Split(TAG_SEPARATOR)
                  .Select(part => part.Trim().ToLower())
                  .Where(part => !string.IsNullOrEmpty(part) && !toDelete.Contains(part));
    }
}
