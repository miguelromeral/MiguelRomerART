using MRA.DTO.Enums.DrawingFilter;
using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art;
using MRA.Services.Models.Drawings;

namespace MRA.Services.Tests.Sorts;

public class AppServiceFilterDrawingsSortsTests : AppServiceTests
{
    public AppServiceFilterDrawingsSortsTests()
    {
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_Latest()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", DateObject = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                new DrawingModel { Id = "3", DateObject = new DateTime(2020, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                new DrawingModel { Id = "2", DateObject = new DateTime(2023, 1, 12, 0, 0, 0, DateTimeKind.Utc) }
            };
        var expectedDrawings = allDrawings.SortByLatest();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.Latest);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_Oldest()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "3", DateObject = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                new DrawingModel { Id = "1", DateObject = new DateTime(2020, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                new DrawingModel { Id = "2", DateObject = new DateTime(2023, 1, 12, 0, 0, 0, DateTimeKind.Utc) }
            };
        var expectedDrawings = allDrawings.SortByOldest();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.Oldest);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_NameAZ()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "3", Name = "Verstappen" },
                new DrawingModel { Id = "1", Name = "Alonso" },
                new DrawingModel { Id = "2", Name = "Hamilton" }
            };
        var expectedDrawings = allDrawings.SortByNameAZ();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.NameAZ);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_NameZA()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Name = "Verstappen" },
                new DrawingModel { Id = "3", Name = "Alonso" },
                new DrawingModel { Id = "2", Name = "Hamilton" }
            };
        var expectedDrawings = allDrawings.SortByNameZA();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.NameZA);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_LikeAscending()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "3", Likes = 30 },
                new DrawingModel { Id = "1", Likes = 10 },
                new DrawingModel { Id = "2", Likes = 20 }
            };
        var expectedDrawings = allDrawings.SortByLikeAscending();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.LikeAscending);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_LikeDescending()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Likes = 30 },
                new DrawingModel { Id = "3", Likes = 10 },
                new DrawingModel { Id = "2", Likes = 20 }
            };
        var expectedDrawings = allDrawings.SortByLikeDescending();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.LikeDescending);
    }


    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_ViewsAscending()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "3", Views = 30 },
                new DrawingModel { Id = "1", Views = 10 },
                new DrawingModel { Id = "2", Views = 20 }
            };
        var expectedDrawings = allDrawings.SortByViewsAscending();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.ViewsAscending);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_ViewsDescending()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Views = 30 },
                new DrawingModel { Id = "3", Views = 10 },
                new DrawingModel { Id = "2", Views = 20 }
            };
        var expectedDrawings = allDrawings.SortByViewsDescending();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.ViewsDescending);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_AuthorScoreAscending()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "4", ScoreCritic = 70, ScorePopular = 50, VotesPopular = 2 },
                new DrawingModel { Id = "2", ScoreCritic = 70, ScorePopular = 0, VotesPopular = 0 },
                new DrawingModel { Id = "1", ScoreCritic = 50, ScorePopular = 0, VotesPopular = 0 },
                new DrawingModel { Id = "5", ScoreCritic = 70, ScorePopular = 90, VotesPopular = 1 },
                new DrawingModel { Id = "3", ScoreCritic = 70, ScorePopular = 50, VotesPopular = 1 },
            };
        var expectedDrawings = allDrawings.SortByAuthorScoreAscending();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.AuthorScoreAscending);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_AuthorScoreDescending()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "5", ScoreCritic = 60, ScorePopular = 0, VotesPopular = 0 },
                new DrawingModel { Id = "2", ScoreCritic = 90, ScorePopular = 80, VotesPopular = 1 },
                new DrawingModel { Id = "4", ScoreCritic = 90, ScorePopular = 70, VotesPopular = 10 },
                new DrawingModel { Id = "1", ScoreCritic = 90, ScorePopular = 80, VotesPopular = 2 },
                new DrawingModel { Id = "3", ScoreCritic = 90, ScorePopular = 0, VotesPopular = 0 },
            };
        var expectedDrawings = allDrawings.SortByAuthorScoreDescending();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.AuthorScoreDescending);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_UserScoreAscending()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "2", ScorePopular = 50, VotesPopular = 1, ScoreCritic = 60 },
                new DrawingModel { Id = "3", ScorePopular = 50, VotesPopular = 1, ScoreCritic = 80 },
                new DrawingModel { Id = "4", ScorePopular = 50, VotesPopular = 2, ScoreCritic = 50 },
                new DrawingModel { Id = "1", ScorePopular = 0, VotesPopular = 0, ScoreCritic = 50 },
                new DrawingModel { Id = "5", ScorePopular = 70, VotesPopular = 1, ScoreCritic = 90 },
            };
        var expectedDrawings = allDrawings.SortByUserScoreAscending();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.UserScoreAscending);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_UserScoreDescending()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "4", ScorePopular = 70, VotesPopular = 1, ScoreCritic = 80 },
                new DrawingModel { Id = "1", ScorePopular = 90, VotesPopular = 5, ScoreCritic = 95 },
                new DrawingModel { Id = "2", ScorePopular = 90, VotesPopular = 5, ScoreCritic = 80 },
                new DrawingModel { Id = "3", ScorePopular = 90, VotesPopular = 2, ScoreCritic = 95 },
                new DrawingModel { Id = "5", ScorePopular = 0, VotesPopular = 0, ScoreCritic = 50 },
            };
        var expectedDrawings = allDrawings.SortByUserScoreDescending();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.UserScoreDescending);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_Fastest()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "3", Time = 30 },
                new DrawingModel { Id = "1", Time = 10 },
                new DrawingModel { Id = "2", Time = 20 },
            };
        var expectedDrawings = allDrawings.SortByFastest();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.Fastest);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_Slowest()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Time = 30 },
                new DrawingModel { Id = "3", Time = 10 },
                new DrawingModel { Id = "2", Time = 20 },
            };
        var expectedDrawings = allDrawings.SortBySlowest();

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.Slowest);
    }

    [Fact]
    public async Task FilterDrawingsAsync_Ok_SortBy_Popularity()
    {
        var allDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "3", ScoreCritic = 5, PopularityCritic = 50 },
                new DrawingModel { Id = "1", ScoreCritic = 10, PopularityCritic = 100 },
                new DrawingModel { Id = "2", ScoreCritic = 7, PopularityCritic = 70 },
            };
        var expectedDrawings = allDrawings.SortByPopularity();

        _mockRemoteConfigService.Setup(s => s.GetPopularityCritic()).Returns(10);

        await Assert_FilteredSortedResults(allDrawings, expectedDrawings, DrawingFilterSortBy.Popularity);
    }

    private async Task Assert_FilteredSortedResults(IEnumerable<DrawingModel> allDrawings, IEnumerable<DrawingModel> expectedDrawings, DrawingFilterSortBy sortBy)
    {
        var filters = DrawingFilter.GetModelNoFilters();
        filters.Sortby = sortBy;

        MockFilters(expectedDrawings);
        await Assert_FilteredResults(allDrawings.ToList(), expectedDrawings.ToList(), filters);
    }
}
