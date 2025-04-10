using MRA.DTO.Models.Interfaces;
using MRA.Infrastructure.Database;

namespace MRA.DTO.Firebase.Converters.Interfaces;

public interface IFirestoreDocumentConverter<Model, Document> 
    where Model : IModel
    where Document : IDocument
{
    Model ConvertToModel(Document document);

    Document ConvertToDocument(Model model);
}
