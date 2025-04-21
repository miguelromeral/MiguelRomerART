using MRA.Infrastructure.Enums;
using System.ComponentModel;

namespace MRA.DTO.Enums.DrawingFilter;

[DefaultEnumValue(NotSpecified)]
public enum DrawingFilterSortBy
{
    [Description("")]
    NotSpecified,

    [Description("popularity")]
    Popularity,

    [Description("date-asc")]
    Latest,

    [Description("date-desc")]
    Oldest,

    [Description("name-asc")]
    NameAZ,

    [Description("name-desc")]
    NameZA,

    [Description("kudos-asc")]
    LikeAscending,

    [Description("kudos-desc")]
    LikeDescending,

    [Description("views-asc")]
    ViewsAscending,

    [Description("views-desc")]
    ViewsDescending,

    [Description("scorem-asc")]
    AuthorScoreAscending,

    [Description("scorem-desc")]
    AuthorScoreDescending,

    [Description("scoreu-asc")]
    UserScoreAscending,

    [Description("scoreu-desc")]
    UserScoreDescending,

    [Description("time-asc")]
    Fastest,

    [Description("time-desc")]
    Slowest,
}
