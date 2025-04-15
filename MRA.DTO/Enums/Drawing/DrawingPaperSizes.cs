using MRA.Infrastructure.Enums;
using System.ComponentModel;

namespace MRA.DTO.Enums.Drawing;

[DefaultEnumValue(NotSpecified)]
public enum DrawingPaperSizes
{
    [Description("Not Specified")]
    NotSpecified = -1,

    [Description("Unknown")]
    Unknown = 0,

    [Description("A1")]
    A1 = 1,

    [Description("A2")]
    A2 = 2,

    [Description("A3")]
    A3 = 3,

    [Description("A4")]
    A4 = 4,

    [Description("A5")]
    A5 = 5,

    [Description("A6")]
    A6 = 6,
}
