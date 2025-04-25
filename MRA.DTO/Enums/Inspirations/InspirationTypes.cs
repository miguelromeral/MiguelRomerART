using MRA.Infrastructure.Enums;
using System.ComponentModel;

namespace MRA.DTO.Enums.Inspirations;

[DefaultEnumValue(Others)]
public enum InspirationTypes
{
    [Description("Others")] 
    Others = 0,

    [Description("Professionals")]
    Professionals = 1,

    [Description("Amateurs")]
    Amateurs = 2,

    [Description("Models")]
    Models = 3,
}