using Google.Cloud.Firestore;
using Moq;
using MRA.DTO.Firebase.Documents;
using MRA.DTO.Firebase.Models;
using MRA.Services.Firebase.Firestore;
using MRA.Services.Firebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using MRA.DTO.Logger;

namespace MRA.Test.Services
{
    public class FirestoreServiceTest : TestBase
    {
        private readonly FirestoreService _firestoreService;
        private readonly Mock<IFirestoreDatabase> _firestoreDbMock;

        public FirestoreServiceTest()
        {
            _firestoreDbMock = new Mock<IFirestoreDatabase>();
            _firestoreService = new FirestoreService(Configuration, _firestoreDbMock.Object, new MRLogger(Configuration));
        }

        [Fact]
        public void ShouldInitialize_WithCorrectConfiguration()
        {
            Assert.Equal("TestProjectID", _firestoreService.ProjectId);
            Assert.Equal("TestDrawings", _firestoreService.CollectionDrawings);
            Assert.Equal("TestAzureUrl", _firestoreService.AzureUrlBase);
            Assert.False(_firestoreService.IsInProduction);
        }

        [Fact]
        public async Task GetDrawingsAsync_ShouldReturnListOfDrawings()
        {
            _firestoreDbMock.Setup(db => db.GetAllDocumentsAsync<DrawingDocument>(_firestoreService.CollectionDrawings)).ReturnsAsync(new List<DrawingDocument>()
            {
                new DrawingDocument(), 
                new DrawingDocument()
            });

            // Actúa: llama al método `GetDrawingsAsync`
            var result = await _firestoreService.GetDrawingsAsync();

            // Verifica que se devolvió la lista de `Drawing` y que tiene la cantidad esperada
            Assert.IsType<List<Drawing>>(result);
            Assert.NotEmpty(result);
        }
    }
}
