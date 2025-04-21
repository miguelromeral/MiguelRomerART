using Moq;
using MRA.DTO.Enums.Drawing;
using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art;

namespace MRA.Services.Tests.Filters;

public class AppServiceFilterDrawingsFiltersTests : AppServiceTests
{
    public AppServiceFilterDrawingsFiltersTests()
    {
    }


    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_Visible()
    {
        var onlyIfVisible = true;
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Visible = true },
                new DrawingModel { Id = "2", Visible = false }
            };
        var expectedDrawings = allDrawings.Where(d => d.Visible).ToList();
        var filters = DrawingFilter.GetModelNoFilters();
        filters.OnlyVisible = onlyIfVisible;

        MockFilters(expectedDrawings);

        var result = await _mockAppService.Object.FilterDrawingsAsync(filters);

        Assert.NotNull(result);

        var resultedDrawings = result.FilteredDrawings;
        Assert.NotEqual(allDrawings, resultedDrawings);
        Assert.Equal(expectedDrawings, resultedDrawings);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_Favorite()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Favorite = true },
                new DrawingModel { Id = "2", Favorite = false }
            };
        var expectedDrawings = allDrawings.Where(d => d.Favorite).ToList();
        var filters = DrawingFilter.GetModelNoFilters();
        filters.Favorites = true;

        MockFilters(expectedDrawings);

        var result = await _mockAppService.Object.FilterDrawingsAsync(filters);

        Assert.NotNull(result);

        var resultedDrawings = result.FilteredDrawings;
        Assert.NotEqual(allDrawings, resultedDrawings);
        Assert.Equal(expectedDrawings, resultedDrawings);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_Tags()
    {
        var filteredTag = "tag1";
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Tags = new List<string>(){ "tag1", "tag2" } },
                new DrawingModel { Id = "2", Tags = new List<string>(){ "tag3", "tag4" } }
            };
        var expectedDrawings = allDrawings.Where(d => d.Tags.Contains(filteredTag)).ToList();
        var filters = DrawingFilter.GetModelNoFilters();
        filters.TextQuery = filteredTag;

        MockFilters(expectedDrawings);
        _mockDrawingService
            .Setup(x => x.DeleteAndAdjustTags(It.IsAny<IEnumerable<string>>()))
            .Returns((List<string> tags) => tags);

        var result = await _mockAppService.Object.FilterDrawingsAsync(filters);

        Assert.NotNull(result);

        var resultedDrawings = result.FilteredDrawings;
        Assert.NotEqual(allDrawings, resultedDrawings);
        Assert.Equal(expectedDrawings, resultedDrawings);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_ProductName_None()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", ProductName = "" },
                new DrawingModel { Id = "2", ProductName = "Awesome Movie" }
            };
        var expectedDrawings = allDrawings.Where(d => string.IsNullOrEmpty(d.ProductName)).ToList();

        await FilterProductName(allDrawings, DrawingFilter.NoProduct, expectedDrawings);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_ProductName_Specified()
    {
        var filteredProductName = "Awesome Movie";
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", ProductName = filteredProductName },
                new DrawingModel { Id = "2", ProductName = "Awesome Show" }
            };
        var expectedDrawings = allDrawings.Where(d => d.ProductName == filteredProductName).ToList();

        await FilterProductName(allDrawings, filteredProductName, expectedDrawings);
    }

    private async Task FilterProductName(List<DrawingModel> allDrawings, string filteredProductName, List<DrawingModel> expectedDrawings)
    {
        var filters = DrawingFilter.GetModelNoFilters();
        filters.ProductName = filteredProductName;

        MockFilters(expectedDrawings);
        await Assert_FilteredResults(allDrawings, expectedDrawings, filters);
    }


    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_CharactertName_None()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Name = "" },
                new DrawingModel { Id = "2", Name = "Cloud Strife" }
            };
        var expectedDrawings = allDrawings.Where(d => string.IsNullOrEmpty(d.Name)).ToList();

        await FilterCharacterName(allDrawings, DrawingFilter.NoCharacter, expectedDrawings);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_CharacterName_Specified()
    {
        var filteredCharacterName = "Cloud Strife";
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", ProductName = filteredCharacterName },
                new DrawingModel { Id = "2", ProductName = "Tifa Lockhart" }
            };
        var expectedDrawings = allDrawings.Where(d => d.Name == filteredCharacterName).ToList();

        await FilterCharacterName(allDrawings, filteredCharacterName, expectedDrawings);
    }

    private async Task FilterCharacterName(List<DrawingModel> allDrawings, string filteredCharacterName, List<DrawingModel> expectedDrawings)
    {
        var filters = DrawingFilter.GetModelNoFilters();
        filters.CharacterName = filteredCharacterName;

        MockFilters(expectedDrawings);
        await Assert_FilteredResults(allDrawings, expectedDrawings, filters);
    }


    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_ModelName_None()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", ModelName = "" },
                new DrawingModel { Id = "2", ModelName = "Miguel" }
            };
        var expectedDrawings = allDrawings.Where(d => string.IsNullOrEmpty(d.ModelName)).ToList();

        await FilterModelName(allDrawings, DrawingFilter.NoCharacter, expectedDrawings);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_ModelName_Specified()
    {
        var filteredCharacterName = "Miguel";
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", ProductName = filteredCharacterName },
                new DrawingModel { Id = "2", ProductName = "Chechu" }
            };
        var expectedDrawings = allDrawings.Where(d => d.Name == filteredCharacterName).ToList();

        await FilterModelName(allDrawings, filteredCharacterName, expectedDrawings);
    }

    private async Task FilterModelName(List<DrawingModel> allDrawings, string filteredModelName, List<DrawingModel> expectedDrawings)
    {
        var filters = DrawingFilter.GetModelNoFilters();
        filters.ModelName = filteredModelName;

        MockFilters(expectedDrawings);
        await Assert_FilteredResults(allDrawings, expectedDrawings, filters);
    }


    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_Type()
    {
        var filteredType = DrawingTypes.GraphitePencils;
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Type = filteredType },
                new DrawingModel { Id = "2", Type = DrawingTypes.Digital }
            };
        var expectedDrawings = allDrawings.Where(d => d.Type == filteredType).ToList();

        var filters = DrawingFilter.GetModelNoFilters();
        filters.Type = filteredType;

        MockFilters(expectedDrawings);
        await Assert_FilteredResults(allDrawings, expectedDrawings, filters);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_ProductType()
    {
        var filteredProductType = DrawingProductTypes.ActorActress;
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", ProductType = filteredProductType },
                new DrawingModel { Id = "2", ProductType = DrawingProductTypes.Influencer }
            };
        var expectedDrawings = allDrawings.Where(d => d.ProductType == filteredProductType).ToList();

        var filters = DrawingFilter.GetModelNoFilters();
        filters.ProductType = filteredProductType;

        MockFilters(expectedDrawings);
        await Assert_FilteredResults(allDrawings, expectedDrawings, filters);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_Software()
    {
        var filteredSoftware = DrawingSoftwares.ClipStudioPaint;
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Software = filteredSoftware },
                new DrawingModel { Id = "2", Software = DrawingSoftwares.MedibangPaint }
            };
        var expectedDrawings = allDrawings.Where(d => d.Software == filteredSoftware).ToList();

        var filters = DrawingFilter.GetModelNoFilters();
        filters.Software = filteredSoftware;

        MockFilters(expectedDrawings);
        await Assert_FilteredResults(allDrawings, expectedDrawings, filters);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_Paper()
    {
        var filteredPaper = DrawingPaperSizes.A4;
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Paper = filteredPaper },
                new DrawingModel { Id = "2", Paper = DrawingPaperSizes.Unknown }
            };
        var expectedDrawings = allDrawings.Where(d => d.Paper == filteredPaper).ToList();

        var filters = DrawingFilter.GetModelNoFilters();
        filters.Paper = filteredPaper;

        MockFilters(expectedDrawings);
        await Assert_FilteredResults(allDrawings, expectedDrawings, filters);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_Spotify_None()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", SpotifyUrl = string.Empty },
                new DrawingModel { Id = "2", SpotifyUrl = "https://open.spotify.com/track/1C2QJNTmsTxCDBuIgai8QV" }
            };
        var expectedDrawings = allDrawings.Where(d => string.IsNullOrEmpty(d.SpotifyUrl)).ToList();

        await FilterSpotify(allDrawings, false, expectedDrawings);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_Spotify_Specified()
    {
        var spotifyTrack = "https://open.spotify.com/track/2LwsunYgfRoqyIsNtgOCQx";
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", SpotifyUrl = spotifyTrack },
                new DrawingModel { Id = "2", SpotifyUrl = string.Empty }
            };
        var expectedDrawings = allDrawings.Where(d => !string.IsNullOrEmpty(d.SpotifyUrl)).ToList();

        await FilterSpotify(allDrawings, true, expectedDrawings);
    }

    private async Task FilterSpotify(List<DrawingModel> allDrawings, bool spotify, List<DrawingModel> expectedDrawings)
    {
        var filters = DrawingFilter.GetModelNoFilters();
        filters.Spotify = spotify;

        MockFilters(expectedDrawings);
        await Assert_FilteredResults(allDrawings, expectedDrawings, filters);
    }


    [Fact]
    public async Task FilterDrawingsAsync_Ok_Filters_Collection()
    {
        var collectionId = "testing-collection";
        var drawingIdInCollection = "1";
        var drawingInCollection = new DrawingModel { Id = drawingIdInCollection };
        var allDrawings = new List<DrawingModel>
            {
                drawingInCollection,
                new DrawingModel { Id = "2" }
            };
        var allCollections = new List<CollectionModel>
            {
                new CollectionModel {
                    Id = collectionId,
                    DrawingIds = new List<string>(){ drawingIdInCollection },
                    Drawings =  new List<DrawingModel>(){ drawingInCollection }
                }
            };
        var expectedDrawings = allDrawings.Where(d => d.Id == drawingIdInCollection).ToList();

        var filters = DrawingFilter.GetModelNoFilters();
        filters.Collection = collectionId;

        MockFiltersWithCollection(expectedDrawings, allCollections);

        await Assert_FilteredResults(allDrawings, expectedDrawings, filters);
    }
}
