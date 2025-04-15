using MRA.Infrastructure.Enums;
using System.ComponentModel;

namespace MRA.DTO.Enums.Drawing;


[DefaultEnumValue(NotSpecified)]
public enum DrawingTypes
{
    [Description("Not Specified")]
    NotSpecified = -1,

    [Description("Others")]
    Others = 0,

    [Description("Graphite Pencils")]
    GraphitePencils = 1,

    [Description("Digital")]
    Digital = 2,

    [Description("Sketch")]
    Sketch = 3,

    [Description("Markers")]
    Markers = 4,

    [Description("Color Pencils")]
    ColorPencils = 5,

    [Description("Pen")]
    Pen = 6,

    [Description("Line Art")]
    LineArt = 7
}