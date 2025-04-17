using MRA.DTO.Models.Interfaces;

namespace MRA.Services.Models.Documents;

public interface IDocumentModelService<Model>
    where Model : IModel
{
    Task<IEnumerable<Model>> GetAllAsync();
    Task<bool> ExistsAsync(string id);
    Task<Model> FindAsync(string id);
    Task<bool> SetAsync(string id, Model model);
    Task<bool> DeleteAsync(string id);
}
