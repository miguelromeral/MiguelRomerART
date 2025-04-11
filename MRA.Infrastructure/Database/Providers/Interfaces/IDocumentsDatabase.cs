using Google.Cloud.Firestore;
using MRA.Infrastructure.Database.Documents.Interfaces;

namespace MRA.Infrastructure.Database.Providers.Interfaces;

public interface IDocumentsDatabase
{
    Task<IEnumerable<IDocument>> GetAllDocumentsAsync<IDocument>(string collection);
    
    Task<IDocument> GetDocumentAsync<IDocument>(string collection, string documentId);
    Task<bool> DocumentExistsAsync(string collection, string documentId);
    
    Task<bool> SetDocumentAsync(string collection, string documentId, IDocument document);
    
    Task<bool> DeleteDocumentAsync(string collection, string id);
}
