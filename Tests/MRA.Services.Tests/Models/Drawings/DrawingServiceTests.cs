using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Documents.MongoDb;
using MRA.Infrastructure.Settings.Options;
using MRA.Infrastructure.Settings;
using MRA.Services.Tests.Models.Base;
using static MRA.Infrastructure.Settings.Options.DatabaseSettings;
using MRA.Services.Models.Drawings;
using MRA.DTO.Models;
using MRA.DTO.Exceptions;
using MRA.DTO.Enums.Drawing;
using MRA.DTO.ViewModels.Art.Select;
using static MRA.Infrastructure.Settings.Options.DatabaseSettings.DatabaseDrawingsTagsOptions;

namespace MRA.Services.Tests.Models.Drawings;

public class DrawingServiceTests : DocumentModelServiceTestsBase
{
    private readonly DrawingService _service;
    private readonly AppSettings _appSettings;
    protected override string COLLECTION_NAME { get => "drawings"; }

    public DrawingServiceTests() : base()
    {
        _appSettings = new AppSettings
        {
            Database = new DatabaseSettings
            {
                Collections = new DatabaseCollectionsOptions
                {
                    Drawings = COLLECTION_NAME
                },
                Drawings = new DatabaseDrawingsOptions
                {
                    Tags = new DatabaseDrawingsTagsOptions
                    {
                        Delete = new List<string>() { "delete", "my" },
                        Replace = new List<DatabaseDrawingsTagsReplaceOptions>
                        {
                            new DatabaseDrawingsTagsReplaceOptions{ Key = "á", Value = "a" },
                            new DatabaseDrawingsTagsReplaceOptions{ Key = "é", Value = "e" },
                        }
                    }
                }
            },
            AzureStorage = new AzureStorageSettings
            {
                BlobPath = "/blobpath"
            }
        };

        _service = new DrawingService(_appSettings, _mockDb.Object);
    }

    [Fact]
    public async Task GetProductsAsync_Ok()
    {
        var drawingDocuments = GetMockDrawingsDocumentsForListItems();
        var expectedResults = new List<ProductListItem>()
        {
            new ProductListItem("The Last Of Us", DrawingProductTypes.Videogame),
        };

        MockGetAllDocuments(drawingDocuments);

        var results = await _service.GetProductsAsync();

        Assert.NotNull(results);
        Assert.Equal(expectedResults.Count, results.Count());

        Assert.Equal(expectedResults, results);
    }

    [Fact]
    public async Task GetCharactersAsync_Ok()
    {
        var drawingDocuments = GetMockDrawingsDocumentsForListItems();
        var expectedResults = new List<CharacterListItem>()
        {
            new CharacterListItem("Joel Miller", DrawingProductTypes.Videogame),
            new CharacterListItem("Ellie Williams", DrawingProductTypes.ActorActress),
            new CharacterListItem("Abby Anderson", DrawingProductTypes.ActorActress),
        };

        MockGetAllDocuments(drawingDocuments);

        var results = await _service.GetCharactersAsync();

        Assert.NotNull(results);
        Assert.Equal(expectedResults, results);
    }

    [Fact]
    public async Task GetModelsAsync_Ok()
    {
        var drawingDocuments = GetMockDrawingsDocumentsForListItems();
        var expectedResults = new List<ModelListItem>()
        {
            new ModelListItem("Troy Baker"),
            new ModelListItem("Ashley Johnson"),
            new ModelListItem("Bella Ramsey"),
            new ModelListItem("Kaitlyn Denver"),
        };

        MockGetAllDocuments(drawingDocuments);

        var results = await _service.GetModelsAsync();

        Assert.NotNull(results);
        Assert.Equivalent(expectedResults, results);
    }

    private static IEnumerable<IDrawingDocument> GetMockDrawingsDocumentsForListItems()
    {
        var productName = "The Last Of Us";
        return new List<IDrawingDocument>
        {
            new DrawingMongoDocument
            {
                Id = "joel",
                name = "Joel Miller",
                model_name = "Troy Baker",
                product_name = productName,
                product_type = (int) DrawingProductTypes.Videogame,
            },
            new DrawingMongoDocument
            {
                Id = "ellie",
                name = "Ellie Williams",
                model_name = "Ashley Johnson",
                product_name = productName,
                product_type = (int) DrawingProductTypes.Videogame,
            },
            new DrawingMongoDocument
            {
                Id = "ellie",
                name = "Ellie Williams",
                model_name = "Bella Ramsey",
                product_name = productName,
                product_type = (int) DrawingProductTypes.ActorActress,
            },
            new DrawingMongoDocument
            {
                Id = "abby",
                name = "Abby Anderson",
                model_name = "Kaitlyn Denver",
                product_name = productName,
                product_type = (int) DrawingProductTypes.ActorActress,
            },
        };
    }

    [Fact]
    public async Task GetAllDrawingsAsync_Ok_OnlyVisibles()
    {
        var drawingDocuments = new List<IDrawingDocument>
        {
            new DrawingMongoDocument
            {
                Id = "1",
                visible = true,
            },
            new DrawingMongoDocument
            {
                Id = "2",
                visible = false
            },
        };

        MockGetAllDocuments(drawingDocuments);

        var result = await _service.GetAllDrawingsAsync(onlyIfVisible: true);

        Assert.NotNull(result);
        Assert.Single(result);

        var first = result.First();
        Assert.Equal("1", first.Id);
    }

    [Fact]
    public async Task GetAllDrawingsAsync_Ok_FullList()
    {
        var drawingDocuments = new List<IDrawingDocument>
        {
            new DrawingMongoDocument
            {
                Id = "1",
                visible = true,
            },
            new DrawingMongoDocument
            {
                Id = "2",
                visible = false
            },
        };

        MockGetAllDocuments(drawingDocuments);

        var result = await _service.GetAllDrawingsAsync(onlyIfVisible: false);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        var first = result.First();
        Assert.Equal("1", first.Id);
    }


    [Fact]
    public async Task FindDrawingAsync_Ok_OnlyVisible()
    {
        var expectedDrawing = new DrawingModel
        {
            Id = "1",
            Visible = true,
        };
        var drawingDocument = new DrawingMongoDocument
        {
            Id = expectedDrawing.Id,
            visible = expectedDrawing.Visible,
            views = expectedDrawing.Views
        };

        MockDocumentExists(true, expectedDrawing.Id);
        MockFindDocument<IDrawingDocument>(drawingDocument, drawingDocument.Id);

        var result = await _service.FindDrawingAsync(expectedDrawing.Id, onlyIfVisible: true, updateViews: false);

        Assert.NotNull(result);
        Assert.Equal(expectedDrawing.Id, result.Id);
        Assert.True(result.Visible);
    }

    [Fact]
    public async Task FindDrawingAsync_Error_NotExists()
    {
        var expectedDrawing = new DrawingModel
        {
            Id = "non-existing",
            Visible = false,
        };

        MockDocumentExists(false, expectedDrawing.Id);

        await Assert.ThrowsAsync<DrawingNotFoundException>(() => _service.FindDrawingAsync(expectedDrawing.Id, onlyIfVisible: true, updateViews: false));
    }


    [Fact]
    public async Task FindDrawingAsync_Error_OnlyVisible()
    {
        var expectedDrawing = new DrawingModel
        {
            Id = "1",
            Visible = false,
        };
        var drawingDocument = new DrawingMongoDocument
        {
            Id = expectedDrawing.Id,
            visible = expectedDrawing.Visible,
        };

        MockDocumentExists(true, expectedDrawing.Id);
        MockFindDocument<IDrawingDocument>(drawingDocument, drawingDocument.Id);

        await Assert.ThrowsAsync<DrawingNotFoundException>(() => _service.FindDrawingAsync(expectedDrawing.Id, onlyIfVisible: true, updateViews: false));
    }

    [Fact]
    public async Task FindDrawingAsync_Ok_UpdateViews()
    {
        var expectedDrawing = new DrawingModel
        {
            Id = "1",
            Views = 10
        };
        var drawingDocument = new DrawingMongoDocument
        {
            Id = expectedDrawing.Id,
            views = expectedDrawing.Views
        };

        MockDocumentExists(true, expectedDrawing.Id);
        MockFindDocument<IDrawingDocument>(drawingDocument, drawingDocument.Id);
        MockSetDocument(true, expectedDrawing.Id);

        var result = await _service.FindDrawingAsync(expectedDrawing.Id, onlyIfVisible: false, updateViews: true);

        Assert.NotNull(result);
        Assert.Equal(expectedDrawing.Id, result.Id);
        Assert.Equal(expectedDrawing.Views + 1, result.Views);
    }


    [Fact]
    public async Task VoteDrawingAsync_Ok_NotVotedYet()
    {
        var drawingId = "not-voted-yet";
        var newScore = 50;
        var drawingDocument = new DrawingMongoDocument
        {
            Id = drawingId,
        };

        MockDocumentExists(true, drawingId);
        MockFindDocument<IDrawingDocument>(drawingDocument, drawingId);
        MockSetDocument(true, drawingId);

        var result = await _service.VoteDrawingAsync(drawingId, newScore);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(newScore, result.NewScore);
        Assert.Equal(1, result.NewVotes);
    }

    [Fact]
    public async Task VoteDrawingAsync_Ok_AdjustVoteMaxLimit()
    {
        var drawingId = "not-voted-yet";
        var newScore = 200;
        var drawingDocument = new DrawingMongoDocument
        {
            Id = drawingId,
        };

        MockDocumentExists(true, drawingId);
        MockFindDocument<IDrawingDocument>(drawingDocument, drawingId);
        MockSetDocument(true, drawingId);

        var result = await _service.VoteDrawingAsync(drawingId, newScore);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(100, result.NewScore);
        Assert.Equal(1, result.NewVotes);
    }

    [Fact]
    public async Task VoteDrawingAsync_Ok_AdjustVoteMinLimit()
    {
        var drawingId = "voted-once";
        var newScore = -100;
        var drawingDocument = new DrawingMongoDocument
        {
            Id = drawingId,
            votes_popular = 1,
            score_popular = 100
        };

        MockDocumentExists(true, drawingId);
        MockFindDocument<IDrawingDocument>(drawingDocument, drawingId);
        MockSetDocument(true, drawingId);

        var result = await _service.VoteDrawingAsync(drawingId, newScore);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(50, result.NewScore);
        Assert.Equal(2, result.NewVotes);
    }


    [Fact]
    public async Task VoteDrawingAsync_Ok_PreviousVotes()
    {
        var drawingId = "voted-once";
        var newScore = 65;
        var drawingDocument = new DrawingMongoDocument
        {
            Id = drawingId,
            votes_popular = 1,
            score_popular = 100
        };
        var expectedVotes = drawingDocument.votes_popular + 1;
        var expectedScore = DrawingVoteManager.CalculateUserScore(newScore, 
            scorePopular: drawingDocument.score_popular, 
            currentVotes: drawingDocument.votes_popular);

        MockDocumentExists(true, drawingId);
        MockFindDocument<IDrawingDocument>(drawingDocument, drawingId);
        MockSetDocument(true, drawingId);

        var result = await _service.VoteDrawingAsync(drawingId, newScore);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(expectedScore, result.NewScore);
        Assert.Equal(expectedVotes, result.NewVotes);
        Assert.Equal(DrawingModel.CalculateScorePopular(expectedScore), result.NewScoreHuman);
    }

    [Fact]
    public async Task UpdateLikesAsync_Ok()
    {
        var drawingDocument = new DrawingMongoDocument
        {
            Id = "not-liked-yet",
            likes = 0
        };

        MockDocumentExists(true, drawingDocument.Id);
        MockFindDocument<IDrawingDocument>(drawingDocument, drawingDocument.Id);

        var result = await _service.UpdateLikesAsync(drawingDocument.Id);

        Assert.NotNull(result);
        Assert.Equal(drawingDocument.Id, result.Id);
        Assert.Equal(drawingDocument.likes + 1, result.Likes);
    }

    [Fact]
    public async Task UpdateLikesAsync_Error_NotExists()
    {
        var drawingId = "not-existing";
        MockDocumentExists(false, drawingId);

        await Assert.ThrowsAsync<DrawingNotFoundException>(() => _service.UpdateLikesAsync(drawingId));
    }

    [Fact]
    public async Task ExistsDrawing_Ok()
    {
        var drawingId = "existing";
        MockDocumentExists(true, drawingId);

        var result = await _service.ExistsDrawing(drawingId);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsDrawing_Error_NotExists()
    {
        var drawingId = "existing";
        MockDocumentExists(false, drawingId);

        var result = await _service.ExistsDrawing(drawingId);

        Assert.False(result);
    }

    [Fact]
    public async Task SaveDrawingAsync_Ok()
    {
        var name = "Cloud Strife";
        var modelName = "Miguel Romeral";
        var title = "Behold my áwesomé cosplay";
        var productName = "FINAL FANTASY VII";
        var drawing = new DrawingModel
        {
            Id = "1",
            Name = name,
            ModelName = modelName,
            Title = title,
            Software = (int) DrawingSoftwares.ClipStudioPaint,
            Paper = (int) DrawingPaperSizes.A4,
            Type = (int) DrawingTypes.Digital,
            ProductType = (int) DrawingProductTypes.Videogame,
            ProductName = productName,
        };

        MockSetDocument(true, drawing.Id);

        var result = await _service.SaveDrawingAsync(drawing);

        Assert.NotNull(result);
        Assert.Equal(drawing.Id, result.Id);
        Assert.NotEmpty(result.Tags);

        foreach(var deleted in _appSettings.Database.Drawings.Tags.Delete)
        {
            Assert.DoesNotContain(deleted, result.Tags);
        }
        foreach(var replaced in _appSettings.Database.Drawings.Tags.Replace)
        {
            foreach (var tag in result.Tags)
            {
                Assert.DoesNotContain(replaced.Key, tag);
            }
        }
    }

    [Fact]
    public void DeleteAndAdjustTags_Ok()
    {
        var tags = new List<string>()
        {
            "delete", "hello", "áwesome", "gréat", "deleteeverithing"
        };
        var expectedTags = new List<string>()
        {
            "hello", "awesome", "great", "deleteeverithing"
        };

        var result = _service.DeleteAndAdjustTags(tags);

        Assert.NotNull(result);
        Assert.Equal(expectedTags, result);

        var awesomeText = result.ElementAt(2);
        Assert.DoesNotContain("á", awesomeText);
    }
}
