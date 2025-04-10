using MRA.DTO.Models;

namespace MRA.Services.Models.Documents;

public interface IDocumentModelService<Model>
    where Model : IModel
{
    Task<IEnumerable<Model>> GetAllAsync();
    Task<bool> ExistsAsync(string id);
    Task<Model> FindAsync(string id);
    Task<bool> SetAsync(string id, Model document);
    Task<bool> DeleteAsync(string id);
}
