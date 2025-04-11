using MRA.DTO.Models.Interfaces;
using MRA.Infrastructure.Database.Documents.Interfaces;

namespace MRA.DTO.Mapper.Interfaces;

public interface IDocumentMapper<Model, Document> 
    where Model : IModel
    where Document : IDocument
{
    Model ConvertToModel(Document document);

    Document ConvertToDocument(Model model);
}
