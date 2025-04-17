//using MongoDB.Driver.Core.Configuration;
//using MRA.DTO.Mapper.Interfaces;
//using MRA.DTO.Models;
//using MRA.Infrastructure.Database.Documents.Interfaces;
//using MRA.Infrastructure.Database.Providers.Interfaces;
//using MRA.Infrastructure.Settings.Options;
//using MRA.Infrastructure.Settings;
//using MRA.Services.Models.Inspirations;
//using Moq;

//namespace MRA.Services.Tests.Models.Inspirations;

//public class InspirationServiceTests
//{
//    [Fact]
//    public async Task GetAllInspirationsAsync_ReturnsExpectedInspirations()
//    {
//        // Arrange
//        var expectedInspirations = new List<InspirationModel>
//        {
//            new InspirationModel { /* Inicializa propiedades */ },
//            new InspirationModel { /* Otra */ }
//        };

//        var mapperMock = new Mock<IDocumentMapper<InspirationModel, IInspirationDocument>>();
//        var dbMock = new Mock<IDocumentsDatabase>();

//        var appSettings = new AppSettings
//        {
//            Database = new DatabaseSettings
//            {
//                Collections = new CollectionSettings
//                {
//                    Inspirations = "InspirationsCollection"
//                }
//            }
//        };

//        // Creamos un mock de la clase base para simular GetAllAsync()
//        var serviceMock = new Mock<InspirationService>(appSettings, mapperMock.Object, dbMock.Object);
//        serviceMock
//            .Protected()
//            .Setup<Task<IEnumerable<InspirationModel>>>("GetAllAsync")
//            .ReturnsAsync(expectedInspirations);

//        var service = serviceMock.Object;

//        // Act
//        var result = await service.GetAllInspirationsAsync();

//        // Assert
//        Assert.Equal(expectedInspirations, result);
//    }
//}
