using MRA.DTO.Models;
using MRA.Infrastructure.Database;

namespace MRA.DTO.Firebase.Converters;

public interface IFirestoreDocumentConverter<Model, Document> 
    where Model : IModel
    where Document : IDocument
{
    Model ConvertToModel(Document document);

    Document ConvertToDocument(Model model);
}
