using MRA.Infrastructure.Settings;
using MRA.Infrastructure.Storage;
using MRA.Infrastructure.Storage.Connection;
using MRA.Infrastructure.Settings.Options;
using Moq;
using Azure.Storage.Blobs.Models;
using SixLabors.ImageSharp;

namespace MRA.Infrastructure.Tests.Storage
{
    public class AzureStorageProviderTests
    {
        private const string BlobContainer = "test-container";
        private const string TestImagePath = "test-image.jpg";
        private const string BlobPath = "https://test.blob.core.windows.net";
        private readonly Mock<IAzureStorageConnection> _mockConnection;
        private readonly AzureStorageProvider _storageProvider;

        public AzureStorageProviderTests()
        {
            var mockSettings = new AppSettings
            {
                AzureStorage = new AzureStorageSettings
                {
                    BlobStorageContainer = BlobContainer,
                    BlobPath = BlobPath
                }
            };

            _mockConnection = new Mock<IAzureStorageConnection>();

            _storageProvider = new AzureStorageProvider(mockSettings, _mockConnection.Object);
        }

        [Fact]
        public async Task ExistsBlob_Ok_Exists()
        {
            var blobPath = "/path/to/blob";

            _mockConnection
                .Setup(c => c.BlobExists(BlobContainer, blobPath))
                .ReturnsAsync(true);

            var result = await _storageProvider.ExistsBlob(blobPath);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistsBlob_Ok_NotExists()
        {
            var blobPath = "/path/to/blob";

            _mockConnection
                .Setup(c => c.BlobExists(BlobContainer, blobPath))
                .ReturnsAsync(false);

            var result = await _storageProvider.ExistsBlob(blobPath);

            Assert.False(result);
        }

        [Fact]
        public void GetBlobURL_Ok()
        {
            var result = _storageProvider.GetBlobURL();

            Assert.NotNull(result);
            Assert.Equal(BlobPath, result);
        }

        [Fact]
        public async Task ResizeAndSave_Ok_WithPath()
        {
            var blobName = "resized-image.png";
            var width = 100;

            using var mockImage = new Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(100, 100);

            _mockConnection
                .Setup(c => c.UploadImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MemoryStream>()))
                .ReturnsAsync(Mock.Of<BlobContentInfo>());

            bool result;
            using (var fileStream = new MemoryStream())
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                result = await _storageProvider.ResizeAndSave(TestImagePath, blobName, width);
            }

            Assert.True(result);
            _mockConnection.Verify(c => c.UploadImageAsync(BlobContainer, blobName, It.IsAny<MemoryStream>()), Times.Once);
        }

        [Fact]
        public async Task ResizeAndSave_Ok_WithStream()
        {
            var width = 100;

            var inputStream = new MemoryStream();
            using (var fileStream = File.OpenRead(TestImagePath))
            {
                await fileStream.CopyToAsync(inputStream);
            }
            inputStream.Seek(0, SeekOrigin.Begin); // Restablecer la posición del stream al inicio

            _mockConnection
                .Setup(c => c.UploadImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MemoryStream>()))
                .ReturnsAsync(Mock.Of<BlobContentInfo>());

            var result = await _storageProvider.ResizeAndSave(inputStream, TestImagePath, width);

            Assert.True(result);
            _mockConnection.Verify(c => c.UploadImageAsync(BlobContainer, TestImagePath, It.IsAny<MemoryStream>()), Times.Once);
        }

        [Fact]
        public async Task Save_Ok()
        {
            var stream = new MemoryStream();
            var blobLocation = "some/location";

            _mockConnection
                .Setup(c => c.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(Mock.Of<BlobContentInfo>());

            var result = await _storageProvider.Save(stream, blobLocation, TestImagePath);

            Assert.True(result);
            _mockConnection.Verify(c => c.UploadAsync(BlobContainer, TestImagePath, stream), Times.Once);
        }
    
    
        [Fact]
        public void CrearThumbnailName_ReturnsCorrectThumbnailName()
        {
            var imagePath = "images/sample.jpg";

            var result = _storageProvider.CrearThumbnailName(imagePath);

            Assert.Equal("images/sample_tn.png", result);
        }
    }
}