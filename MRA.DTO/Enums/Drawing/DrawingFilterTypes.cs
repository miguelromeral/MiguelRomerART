using MRA.Infrastructure.Enums;
using System.ComponentModel;

namespace MRA.DTO.Enums.Drawing;

[DefaultEnumValue(NotSpecified)]
public enum DrawingFilterTypes
{
    [Description("Not Specified")]
    NotSpecified = -1,

    [Description("Unknown")]
    Unknown = 0,

    [Description("Snapseed")]
    Snapseed = 1,

    [Description("Adobe Photoshop")]
    AdobePhotoshop = 2,

    [Description("Instagram")]
    Instagram = 3,

    [Description("Samsung Galaxy")]
    SamsungGalaxy = 4,
}
