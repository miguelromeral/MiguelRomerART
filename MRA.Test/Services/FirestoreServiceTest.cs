using System.Reflection;
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
using System.Dynamic;
using MRA.Services.Firebase.Interfaces;

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
        public void ShouldInitialize_Ok()
        {
            Assert.Equal("TestProjectID", _firestoreService.ProjectId);
            Assert.Equal("TestDrawings", _firestoreService.CollectionDrawings);
            Assert.Equal("TestAzureUrl", _firestoreService.AzureUrlBase);
            Assert.False(_firestoreService.IsInProduction);
        }

        [Fact]
        public async Task GetDrawingsAsync_Ok()
        {
            _firestoreDbMock.Setup(db => db.GetAllDocumentsAsync<DrawingDocument>(_firestoreService.CollectionDrawings)).ReturnsAsync(new List<DrawingDocument>()
            {
                new DrawingDocument(),
                new DrawingDocument()
            });

            var result = await _firestoreService.GetDrawingsAsync();

            Assert.IsType<List<Drawing>>(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetInspirationsAsync_Ok()
        {
            var expected = new List<InspirationDocument>()
            {
                new InspirationDocument(),
                new InspirationDocument()
            };
            _firestoreDbMock.Setup(db => db.GetAllDocumentsAsync<InspirationDocument>(_firestoreService.CollectionInspirations)).ReturnsAsync(expected);

            var result = await _firestoreService.GetInspirationsAsync();

            Assert.IsType<List<Inspiration>>(result);
            Assert.NotEmpty(result);
            Assert.Equal(expected.Count, result.Count);
        }
    }
}
