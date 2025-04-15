using MRA.Infrastructure.Enums;
using System.ComponentModel;

namespace MRA.DTO.Enums.Drawing;

[DefaultEnumValue(NotSpecified)]
public enum DrawingProductTypes
{
    [Description("Not Specified")]
    NotSpecified = -1,

    [Description("Others")]
    Others = 0,

    [Description("Videogame")]
    Videogame = 1,

    [Description("Actor / Actress")]
    ActorActress = 2,

    [Description("Singer")]
    Singer = 3,

    [Description("Sportman")]
    Sportman = 4,

    [Description("Influencer")]
    Influencer = 5,
}
