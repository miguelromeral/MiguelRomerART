using MRA.DTO.Mapper.Interfaces;
using MRA.DTO.Models.Interfaces;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Providers.Interfaces;

namespace MRA.Services.Models.Documents;

public abstract class DocumentModelService<Model, Document> : IDocumentModelService<Model>
    where Model : IModel
    where Document : IDocument
{
    private readonly string _collectionName;
    private readonly IDocumentsDatabase _db;
    protected IDocumentMapper<Model, Document> Converter;

    protected DocumentModelService(string collectionName, IDocumentMapper<Model, Document> converter, IDocumentsDatabase db)
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
        var documentList = (await _db.GetAllDocumentsAsync<Document>(_collectionName));
        return documentList.Select(Converter.ConvertToModel);
    }

    public async Task<Model> FindAsync(string id)
    {
        var document = await _db.GetDocumentAsync<Document>(_collectionName, id);
        return Converter.ConvertToModel(document);
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
