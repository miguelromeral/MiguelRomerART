using MRA.Infrastructure.Configuration;
using MRA.DTO.Exceptions;
using MRA.DTO.Firebase.Converters;
using MRA.DTO.Firebase.Models;
using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Infrastructure.Database;
using MRA.Infrastructure.Firestore.Documents;
using MRA.Services.Models.Documents;

namespace MRA.Services.Models.Drawings;

public class DrawingService : DocumentModelService<DrawingModel, DrawingDocument>, IDrawingService
{
    public DrawingService(
        AppConfiguration appConfig,
        IFirestoreDocumentConverter<DrawingModel, DrawingDocument> converter,
        IDocumentsDatabase db)
        : base(collectionName: appConfig.Firebase.CollectionDrawings, converter, db)
    {
    }

    public async Task<IEnumerable<DrawingModel>> GetAllDrawingsAsync(bool onlyIfVisible)
    {
        var drawings = await GetAllAsync();

        if (onlyIfVisible)
            drawings = drawings.Where(d => d.Visible);

        return drawings;
    }

    public async Task<DrawingModel> FindDrawingAsync(string id, bool onlyIfVisible, bool updateViews)
    {
        await CheckIfExistsDrawingAsync(id);

        if (updateViews)
            await UpdateViewsAsync(id);

        var drawing = await FindAsync(id);
        if (drawing == null ||
            (onlyIfVisible && !drawing.Visible))
            throw new DrawingNotFoundException(id);

        return drawing;
    }

    public async Task<IEnumerable<ProductListItem>> GetProductsAsync()
    {
        var list = new List<ProductListItem>();
        var drawings = await GetAllAsync();
        foreach (var product in drawings.Where(x => !string.IsNullOrEmpty(x.ProductName)).Select(x => new { x.ProductName, x.ProductType, x.ProductTypeName }).Distinct().ToList())
        {
            if (list.Count(x => x.ProductName == product.ProductName) == 0)
            {
                list.Add(new ProductListItem()
                {
                    ProductName = product.ProductName,
                    ProductTypeId = product.ProductType,
                    ProductType = product.ProductTypeName
                });
            }
        }

        return list;
    }

    public async Task<IEnumerable<CharacterListItem>> GetCharactersAsync()
    {
        var list = new List<CharacterListItem>();
        var drawings = await GetAllAsync();

        foreach (var character in drawings.Where(x => !string.IsNullOrEmpty(x.ProductName)).Select(x => new { x.Name, x.ProductType, x.ProductTypeName }).Distinct().ToList())
        {
            if (list.Count(x => x.CharacterName == character.Name) == 0)
            {
                list.Add(new CharacterListItem()
                {
                    CharacterName = character.Name,
                    ProductTypeId = character.ProductType,
                    ProductType = character.ProductTypeName
                });
            }
        }

        return list;
    }

    public async Task<IEnumerable<string>> GetModelsAsync()
    {
        var list = new List<string>();
        var drawings = await GetAllAsync();

        foreach (var modelName in drawings.Where(x => !string.IsNullOrEmpty(x.ModelName)).Select(x => x.ModelName).Distinct().ToList())
        {
            if (!list.Contains(modelName))
            {
                list.Add(modelName);
            }
        }

        return list;
    }

    public async Task<bool> SaveDrawingAsync(string id, DrawingModel model)
    {
        SetAutomaticTags(ref model);
        return await SetAsync(id, model);
    }

    public async Task<bool> UpdateViewsAsync(string id)
    {
        await CheckIfExistsDrawingAsync(id);

        var drawing = await FindAsync(id);
        drawing.Views++;

        return await SetAsync(id, drawing);
    }

    public async Task<bool> UpdateLikesAsync(string id)
    {
        await CheckIfExistsDrawingAsync(id);

        var drawing = await FindAsync(id);
        drawing.Likes++;

        return await SetAsync(id, drawing);
    }


    public async Task<VoteSubmittedModel> VoteDrawingAsync(string documentId, int score)
    {
        await CheckIfExistsDrawingAsync(documentId);
        
        var votes = new VoteSubmittedModel();
        try
        {
            if (score > 100) score = 100;
            if (score < 0) score = 0;

            var drawing = await FindAsync(documentId);

            if (drawing.VotesPopular > 0)
            {
                votes.NewVotes = drawing.VotesPopular + 1;
                votes.NewScore = ((drawing.ScorePopular * drawing.VotesPopular) + score) / (drawing.VotesPopular + 1);
            }
            else
            {
                votes.NewVotes = 1;
                votes.NewScore = score;
            }

            drawing.VotesPopular = votes.NewVotes;
            drawing.ScorePopular = votes.NewScore;

            votes.Success = await SetAsync(documentId, drawing);
        }
        catch (Exception ex)
        {
            votes.NewVotes = -1;
            votes.Success = false;
        }
        return votes;
    }

    private async Task CheckIfExistsDrawingAsync(string id)
    {
        var exists = await ExistsAsync(id);
        if (!exists)
            throw new DrawingNotFoundException(id);
    }

    public async Task<bool> ExistsDrawing(string id)
    {
        return await ExistsAsync(id);
    }

    public void SetAutomaticTags(ref DrawingModel document)
    {
        var list = new List<string>();
        list.AddRange((document.Name ?? "").Split(" ").Select(x => x.ToLower()));
        list.AddRange((document.ModelName ?? "").Split(" ").Select(x => x.ToLower()));
        list.AddRange((document.Title ?? "").Split(" ").Select(x => x.ToLower()));
        if (document.Software > 0)
        {
            list.AddRange(document.SoftwareName.Split(" ").Select(x => x.ToLower()));
        }
        if (document.Paper > 0)
        {
            list.AddRange(document.PaperHuman.Split(" ").Select(x => x.ToLower()));
        }
        if (document.Type > 0)
        {
            list.AddRange(document.TypeName.Split(" ").Select(x => x.ToLower()));
        }

        if (document.ProductType > 0)
        {
            list.AddRange(document.ProductTypeName.Split(" ").Select(x => x.ToLower()));
        }
        list.AddRange(document.ProductName.Split(" ").Select(x => x.ToLower()));

        document.Tags.AddRange(list);
        document.Tags = DeleteAndAdjustTags(document.Tags).ToList();
    }


    public IEnumerable<string> DeleteAndAdjustTags(IEnumerable<string> tags)
    {
        var eliminar = new List<string>()
            {
                "a",
                "un",
                "unas",
                "unos",
                "uno",
                "de",
                "el",
                "la",
                "los",
                "los",
                "les",
                "the"
            };
        var processed = tags.Select(x =>
            x.ToLower()
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u")
            .Replace("ä", "a")
            .Replace("ë", "e")
            .Replace("ï", "i")
            .Replace("ö", "o")
            .Replace("ü", "u")
            .Replace(":", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("?", "")
            .Replace("¿", "")
            .Replace("/", "")
            .Replace("`", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("'", "")
            .Replace("@", " ")
            .Replace("#", " ")
            .Replace("!", "")
            .Replace("¡", "")
            .Replace("~", "")
            .Replace("$", " ")
            .Replace("%", " ")
            .Replace("&", " ")
            .Replace("\"", "")
            .Replace(" ", "")
            .Replace("_", " ")
            .Replace("-", " ")
        );

        var final = new List<string>() { };
        foreach (var s in processed)
        {
            foreach (var s2 in s.Split(" "))
            {
                if (!eliminar.Contains(s2) && !String.IsNullOrEmpty(s2) && !s2.Equals(" "))
                {
                    final.Add(s);
                }
            }
        }

        return final.Distinct().ToList();
    }
}
