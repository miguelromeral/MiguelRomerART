using Moq;
using MRA.DTO.Mapper.Interfaces;
using MRA.DTO.Models;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Documents.MongoDb;
using MRA.Infrastructure.Database.Providers.Interfaces;

namespace MRA.Services.Tests.Models.Base;

public class DocumentModelServiceTests
{
    private readonly Mock<IDocumentMapper<InspirationModel, IInspirationDocument>> _mapperMock;
    private readonly Mock<IDocumentsDatabase> _dbMock;
    private readonly string _collectionName = "TestCollection";
    private TestDocumentModelService _service;


    public DocumentModelServiceTests()
    {
        _mapperMock = new Mock<IDocumentMapper<InspirationModel, IInspirationDocument>>();
        _dbMock = new Mock<IDocumentsDatabase>();
    }

    private void CreateService()
    {
        _service = new TestDocumentModelService(_collectionName, _mapperMock.Object, _dbMock.Object);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenDocumentExists()
    {
        var docId = "123";
        _dbMock.Setup(db => db.DocumentExistsAsync(_collectionName, docId))
               .ReturnsAsync(true);

        CreateService();

        var result = await _service.ExistsAsync(docId);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenDocumentDoesNotExist()
    {
        var docId = "456";
        _dbMock.Setup(db => db.DocumentExistsAsync(_collectionName, docId))
               .ReturnsAsync(false);

        CreateService();

        var result = await _service.ExistsAsync(docId);

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ThrowsException_WhenDatabaseFails()
    {
        var docId = "123";

        _dbMock.Setup(db => db.DocumentExistsAsync(_collectionName, docId))
               .ThrowsAsync(new Exception("DB error"));

        CreateService();

        await Assert.ThrowsAsync<Exception>(() => _service.ExistsAsync(docId));
    }


    [Fact]
    public async Task GetAllAsync_ReturnsMappedModels()
    {
        var documents = new List<IInspirationDocument> { Mock.Of<IInspirationDocument>(), Mock.Of<IInspirationDocument>() };
        var models = new List<InspirationModel> { new(), new() };

        _dbMock.Setup(db => db.GetAllDocumentsAsync<IInspirationDocument>(_collectionName))
               .ReturnsAsync(documents);

        _mapperMock.SetupSequence(m => m.ConvertToModel(It.IsAny<IInspirationDocument>()))
                   .Returns(models[0])
                   .Returns(models[1]);

        CreateService();

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(models[0], result[0]);
        Assert.Equal(models[1], result[1]);
    }

    [Fact]
    public async Task GetAllAsync_ThrowsException_WhenDatabaseFails()
    {
        _dbMock.Setup(db => db.GetAllDocumentsAsync<IInspirationDocument>(_collectionName))
               .ThrowsAsync(new Exception("DB failure"));

        CreateService();

        await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
    }

    [Fact]
    public async Task FindAsync_ReturnsMappedModel()
    {
        var docId = "doc123";
        var document = Mock.Of<IInspirationDocument>();
        var model = new InspirationModel();

        _dbMock.Setup(db => db.GetDocumentAsync<IInspirationDocument>(_collectionName, docId))
               .ReturnsAsync(document);

        _mapperMock.Setup(m => m.ConvertToModel(document))
                   .Returns(model);

        CreateService();

        var result = await _service.FindAsync(docId);

        Assert.Equal(model, result);
    }

    [Fact]
    public async Task FindAsync_ThrowsException_WhenDatabaseFails()
    {
        var docId = "fail";

        _dbMock.Setup(db => db.GetDocumentAsync<IInspirationDocument>(_collectionName, docId))
               .ThrowsAsync(new Exception("DB error"));

        CreateService();

        await Assert.ThrowsAsync<Exception>(() => _service.FindAsync(docId));
    }

    [Fact]
    public async Task FindAsync_ThrowsException_WhenMapperFails()
    {
        var docId = "fail";
        var doc = Mock.Of<IInspirationDocument>();

        _dbMock.Setup(db => db.GetDocumentAsync<IInspirationDocument>(_collectionName, docId))
               .ReturnsAsync(doc);

        _mapperMock.Setup(m => m.ConvertToModel(doc))
                   .Throws(new Exception("Mapping fail"));

        CreateService();

        await Assert.ThrowsAsync<Exception>(() => _service.FindAsync(docId));
    }


    [Fact]
    public async Task SetAsync_ConvertsAndStoresDocument()
    {
        var docId = "doc456";
        var model = new InspirationModel();
        var document = Mock.Of<IInspirationDocument>();

        _mapperMock.Setup(m => m.ConvertToDocument(model))
                   .Returns(document);

        _dbMock.Setup(db => db.SetDocumentAsync(_collectionName, docId, document))
               .ReturnsAsync(true);

        CreateService();

        var result = await _service.SetAsync(docId, model);

        Assert.True(result);
    }

    [Fact]
    public async Task SetAsync_ThrowsException_WhenMapperFails()
    {
        var docId = "set123";
        var model = new InspirationModel();

        _mapperMock.Setup(m => m.ConvertToDocument(model))
                   .Throws(new Exception("Mapping fail"));

        CreateService();

        await Assert.ThrowsAsync<Exception>(() => _service.SetAsync(docId, model));
    }

    [Fact]
    public async Task SetAsync_ThrowsException_WhenDatabaseFails()
    {
        var docId = "set456";
        var model = new InspirationModel();
        var document = Mock.Of<IInspirationDocument>();

        _mapperMock.Setup(m => m.ConvertToDocument(model))
                   .Returns(document);

        _dbMock.Setup(db => db.SetDocumentAsync(_collectionName, docId, document))
               .ThrowsAsync(new Exception("DB write fail"));

        CreateService();

        await Assert.ThrowsAsync<Exception>(() => _service.SetAsync(docId, model));
    }

    [Fact]
    public async Task DeleteAsync_DeletesDocument()
    {
        var docId = "doc789";

        _dbMock.Setup(db => db.DeleteDocumentAsync(_collectionName, docId))
               .ReturnsAsync(true);

        CreateService();

        var result = await _service.DeleteAsync(docId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenDatabaseFails()
    {
        var docId = "del999";

        _dbMock.Setup(db => db.DeleteDocumentAsync(_collectionName, docId))
               .ThrowsAsync(new Exception("DB delete fail"));

        CreateService();

        await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(docId));
    }

}
