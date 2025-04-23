using Moq;
using MRA.Infrastructure.Settings.Options;
using MRA.Infrastructure.Settings;
using MRA.Services.Storage;
using MRA.Infrastructure.Storage;

namespace MRA.Services.Tests.Storage;

public class StorageServiceTests
{
    private readonly AppSettings _appSettings;
    private readonly StorageService _service;
    protected readonly Mock<IStorageProvider> _mockProvider;

    public StorageServiceTests() : base()
    {
        _appSettings = new AppSettings
        {
            AzureStorage = new AzureStorageSettings
            {
                BlobPath = "/path/to/blob"
            }
        };

        _mockProvider = new Mock<IStorageProvider>();

        _service = new StorageService(_appSettings, _mockProvider.Object);
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
}
