using Moq;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Providers.Interfaces;

namespace MRA.Services.Tests.Models.Base;

public abstract class DocumentModelServiceTestsBase
{
    protected readonly Mock<IDocumentsDatabase> _mockDb;
    protected abstract string COLLECTION_NAME { get; }

    protected DocumentModelServiceTestsBase()
    {
        _mockDb = new Mock<IDocumentsDatabase>();
    }

    protected void MockGetAllDocuments<T>(IEnumerable<T> expected)
    {
        _mockDb.Setup(db => db.GetAllDocumentsAsync<T>(COLLECTION_NAME))
               .ReturnsAsync(expected);
    }

    protected void MockDocumentExists(bool expected, string id)
    {
        _mockDb.Setup(db => db.DocumentExistsAsync(COLLECTION_NAME, id))
               .ReturnsAsync(expected);
    }

    protected void MockFindDocument<TDocument>(TDocument expected, string id) where TDocument : IDocument
    {
        _mockDb.Setup(db => db.GetDocumentAsync<TDocument>(COLLECTION_NAME, id))
               .ReturnsAsync(expected);
    }

    protected void MockFindDocument<TDocument>(Exception expected, string id) where TDocument : IDocument
    {
        _mockDb.Setup(db => db.GetDocumentAsync<TDocument>(COLLECTION_NAME, id))
               .ThrowsAsync(expected);
    }

    protected void MockSetDocument(bool expected, string id)
    {
        _mockDb.Setup(db => db.SetDocumentAsync(COLLECTION_NAME, id, It.IsAny<IDocument>()))
               .ReturnsAsync(expected);
    }
}
