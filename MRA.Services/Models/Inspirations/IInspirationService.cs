using MRA.DTO.Models;

namespace MRA.Services.Models.Inspirations;

public interface IInspirationService
{
    Task<IEnumerable<InspirationModel>> GetAllInspirationsAsync();
}
