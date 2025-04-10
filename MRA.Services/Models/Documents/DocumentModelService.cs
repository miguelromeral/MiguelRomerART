using MRA.DTO.Firebase.Converters.Interfaces;
using MRA.DTO.Models.Interfaces;
using MRA.Infrastructure.Database;

namespace MRA.Services.Models.Documents;

public abstract class DocumentModelService<Model, Document> : IDocumentModelService<Model>
    where Model : IModel
    where Document : IDocument
{
    private readonly string _collectionName;
    private readonly IDocumentsDatabase _db;
    protected IFirestoreDocumentConverter<Model, Document> Converter;

    public DocumentModelService(string collectionName, IFirestoreDocumentConverter<Model, Document> converter, IDocumentsDatabase db)
    {
        _collectionName = collectionName;
        Converter = converter;
        _db = db;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _db.DocumentExistsAsync(_collectionName, id);
    }

    public async Task<IEnumerable<Model>> GetAllAsync()
    {
        return (await _db.GetAllDocumentsAsync<Document>(_collectionName)).Select(Converter.ConvertToModel);
    }

    public async Task<Model> FindAsync(string id)
    {
        return Converter.ConvertToModel(await _db.GetDocumentAsync<Document>(_collectionName, id));
    }

    public async Task<bool> SetAsync(string id, Model model)
    {
        var document = Converter.ConvertToDocument(model);
        return await _db.SetDocumentAsync(_collectionName, id, document);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await _db.DeleteDocumentAsync(_collectionName, id);
    }
}
