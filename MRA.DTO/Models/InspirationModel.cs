
using MRA.DTO.Enums.Inspirations;
using MRA.DTO.Models.Interfaces;
using MRA.Infrastructure.Enums;

namespace MRA.DTO.Models;

public class InspirationModel : IModel
{   public string Id { get; set; }
    public string Name { get; set; }
    public string Instagram { get; set; }
    public string Twitter { get; set; }
    public string YouTube { get; set; }
    public string Twitch { get; set; }
    public string Pinterest { get; set; }
    public InspirationTypes Type { get; set; }
    public string TypeName { get => Type.GetDescription(); }

    public string GetId() => Id;
}
