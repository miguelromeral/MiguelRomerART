using MRA.Infrastructure.Enums;
using System.ComponentModel;

namespace MRA.DTO.Enums.Drawing;

[DefaultEnumValue(NotSpecified)]
public enum DrawingSoftwares
{
    [Description("Not Specified")]
    NotSpecified = -1,

    [Description("None")]
    None = 0,

    [Description("Medibang Paint")]
    MedibangPaint = 1,

    [Description("Clip Studio Paint")]
    ClipStudioPaint = 2,

    [Description("Adobe Photoshop")]
    AdobePhotoshop = 3,

    [Description("GIMP")]
    GIMP = 4,
}
