using Moq;
using MRA.Services.Storage;
using MRA.Infrastructure.Storage;

namespace MRA.Services.Tests.Storage;

public class StorageServiceTests
{
    private readonly StorageService _service;
    protected readonly Mock<IStorageProvider> _mockProvider;

    public StorageServiceTests() : base()
    {
        _mockProvider = new Mock<IStorageProvider>();

        _service = new StorageService(_mockProvider.Object);
    }

    [Fact]
    public async Task ExistsBlob_Ok_Exists()
    {
        var blobPath = "/path/to/blob";

        _mockProvider.Setup(s => s.ExistsBlob(blobPath)).ReturnsAsync(true);

        var result = await _service.ExistsBlob(blobPath);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsBlob_Ok_NotExists()
    {
        var blobPath = "/path/to/blob";

        _mockProvider.Setup(s => s.ExistsBlob(blobPath)).ReturnsAsync(false);

        var result = await _service.ExistsBlob(blobPath);

        Assert.False(result);
    }

    [Fact]
    public void GetBlobURL_Ok()
    {
        var blobPath = "/path/to/blob";

        _mockProvider.Setup(s => s.GetBlobURL()).Returns(blobPath);

        var result = _service.GetBlobURL();

        Assert.NotNull(result);
        Assert.Equal(blobPath, result);
    }

    [Fact]
    public async Task ResizeAndSave_Ok()
    {
        // Arrange
        var inputStream = new MemoryStream();
        var blobName = "test-resized-image.png";
        var desiredWidth = 100;

        // Act
        await _service.ResizeAndSave(inputStream, blobName, desiredWidth);

        // Assert
        _mockProvider.Verify(
            provider => provider.ResizeAndSave(inputStream, blobName, desiredWidth),
            Times.Once
        );
    }

    [Fact]
    public async Task Save_Ok()
    {
        // Arrange
        var stream = new MemoryStream();
        var blobLocation = "some/location";
        var blobName = "test-file.png";

        // Act
        await _service.Save(stream, blobLocation, blobName);

        // Assert
        _mockProvider.Verify(
            provider => provider.Save(stream, blobLocation, blobName),
            Times.Once
        );
    }

    [Fact]
    public void CrearThumbnailName_Ok()
    {
        // Arrange
        var imagePath = "images/sample.jpg";
        var expectedThumbnailName = "images/sample_tn.png";

        _mockProvider
            .Setup(provider => provider.CrearThumbnailName(imagePath))
            .Returns(expectedThumbnailName);

        // Act
        var result = _service.CrearThumbnailName(imagePath);

        // Assert
        Assert.Equal(expectedThumbnailName, result);
        _mockProvider.Verify(provider => provider.CrearThumbnailName(imagePath), Times.Once);
    }
}
