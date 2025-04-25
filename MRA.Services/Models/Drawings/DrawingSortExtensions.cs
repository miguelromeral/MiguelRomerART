using MRA.DTO.Models;

namespace MRA.Services.Models.Drawings;

public static class DrawingSortExtensions
{
    public static IEnumerable<DrawingModel> SortByLatest(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderBy(x => x.DateObject);
    }

    public static IEnumerable<DrawingModel> SortByOldest(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderByDescending(x => x.DateObject);
    }

    public static IEnumerable<DrawingModel> SortByNameAZ(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderBy(x => x.Name);
    }

    public static IEnumerable<DrawingModel> SortByNameZA(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderByDescending(x => x.Name);
    }

    public static IEnumerable<DrawingModel> SortByLikeAscending(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderBy(x => x.Likes);
    }

    public static IEnumerable<DrawingModel> SortByLikeDescending(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderByDescending(x => x.Likes);
    }

    public static IEnumerable<DrawingModel> SortByViewsAscending(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderBy(x => x.Views);
    }

    public static IEnumerable<DrawingModel> SortByViewsDescending(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderByDescending(x => x.Views);
    }

    public static IEnumerable<DrawingModel> SortByAuthorScoreAscending(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderBy(x => x.ScoreCritic).ThenBy(x => x.ScorePopular).ThenBy(x => x.VotesPopular);
    }

    public static IEnumerable<DrawingModel> SortByAuthorScoreDescending(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderByDescending(x => x.ScoreCritic).ThenByDescending(x => x.ScorePopular).ThenByDescending(x => x.VotesPopular);
    }

    public static IEnumerable<DrawingModel> SortByUserScoreAscending(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.Where(x => x.VotesPopular > 0)
                       .OrderBy(x => x.ScorePopular)
                       .ThenBy(x => x.VotesPopular)
                       .ThenBy(x => x.ScoreCritic);
    }

    public static IEnumerable<DrawingModel> SortByUserScoreDescending(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.Where(x => x.VotesPopular > 0)
                       .OrderByDescending(x => x.ScorePopular)
                       .ThenByDescending(x => x.VotesPopular)
                       .ThenByDescending(x => x.ScoreCritic);
    }

    public static IEnumerable<DrawingModel> SortByFastest(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderBy(x => x.Time);
    }

    public static IEnumerable<DrawingModel> SortBySlowest(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderByDescending(x => x.Time);
    }

    public static IEnumerable<DrawingModel> SortByPopularity(this IEnumerable<DrawingModel> drawings)
    {
        return drawings.OrderByDescending(x => x.Popularity);
    }
}