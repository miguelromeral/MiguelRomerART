using Microsoft.Extensions.Logging;
using Moq;
using MRA.Infrastructure.Settings;
using MRA.Services.Models.Collections;
using MRA.Services.Models.Drawings;
using MRA.Services.Models.Inspirations;
using MRA.Services.RemoteConfig;
using MRA.Services.Storage;
using MRA.DTO.Models;
using MRA.Infrastructure.Cache;
using MRA.DTO.ViewModels.Art;

namespace MRA.Services.Tests;

public class AppServiceTests
{
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly Mock<IInspirationService> _mockInspirationService;
    private readonly Mock<ICollectionService> _mockCollectionService;
    private readonly Mock<ILogger<AppService>> _mockLogger;
    private readonly Mock<ICacheProvider> _mockCacheProvider;
    private readonly AppSettings _mockAppSettings;
    protected readonly Mock<IRemoteConfigService> _mockRemoteConfigService;
    protected readonly Mock<IDrawingService> _mockDrawingService;
    protected readonly Mock<AppService> _mockAppService;

    public AppServiceTests()
    {
        _mockStorageService = new Mock<IStorageService>();
        _mockDrawingService = new Mock<IDrawingService>();
        _mockInspirationService = new Mock<IInspirationService>();
        _mockCollectionService = new Mock<ICollectionService>();
        _mockLogger = new Mock<ILogger<AppService>>();
        _mockRemoteConfigService = new Mock<IRemoteConfigService>();
        _mockCacheProvider = new Mock<ICacheProvider>();
        _mockAppSettings = new AppSettings()
        {
            Cache = new Infrastructure.Settings.Options.CacheSettings()
            {
                RefreshSeconds = 1
            }
        };
        
        // Crear un mock de AppService y configurar el método GetOrSetFromCacheAsync
        _mockAppService = new Mock<AppService>(
            _mockCacheProvider.Object,
            _mockStorageService.Object,
            _mockRemoteConfigService.Object,
            _mockDrawingService.Object,
            _mockInspirationService.Object,
            _mockCollectionService.Object,
            _mockLogger.Object,
            _mockAppSettings
        )
        { CallBase = true };
    }

    [Fact]
    public async Task GetAllDrawings_Ok()
    {
        var expectedDrawings = new List<DrawingModel>
            {
                new DrawingModel { Id = "1", Name = "Drawing1" },
                new DrawingModel { Id = "2", Name = "Drawing2" }
            };

        MockCacheDrawings();
        _mockDrawingService
            .Setup(x => x.GetAllDrawingsAsync(It.IsAny<bool>()))
            .ReturnsAsync(expectedDrawings);

        var result = await _mockAppService.Object.GetAllDrawings(onlyIfVisible: true, cache: true);

        Assert.NotNull(result);
        Assert.Equal(expectedDrawings, result);
    }

    private void MockCacheDrawings()
    {
        MockCache<IEnumerable<DrawingModel>>();
    }


    protected async Task Assert_FilteredResults(IEnumerable<DrawingModel> allDrawings, IEnumerable<DrawingModel> expectedDrawings, DrawingFilter filters)
    {
        var result = await _mockAppService.Object.FilterDrawingsAsync(filters);

        Assert.NotNull(result);

        var resultedDrawings = result.FilteredDrawings;
        Assert.NotEqual(allDrawings, resultedDrawings);
        Assert.Equal(expectedDrawings, resultedDrawings);
    }

    private void MockCacheFilterResults()
    {
        MockCache<FilterResults>();
    }

    protected void MockFilters(IEnumerable<DrawingModel> expectedDrawings)
    {
        MockCacheFilterResults();
        MockExpectedDrawings(expectedDrawings);
        MockExpectedCollections(new List<CollectionModel>());
    }

    protected void MockFiltersWithCollection(IEnumerable<DrawingModel> expectedDrawings, IEnumerable<CollectionModel> expectedCollections)
    {
        MockCacheFilterResults();
        MockExpectedDrawings(expectedDrawings);
        MockExpectedCollections(expectedCollections);
    }


    private void MockExpectedDrawings(IEnumerable<DrawingModel> expectedDrawings)
    {
        _mockDrawingService
            .Setup(x => x.GetAllDrawingsAsync(It.IsAny<bool>()))
            .ReturnsAsync(expectedDrawings);
    }

    private void MockExpectedCollections(IEnumerable<CollectionModel> expectedCollections)
    {
        _mockCollectionService
            .Setup(x => x.GetAllCollectionsAsync(It.IsAny<bool>()))
            .ReturnsAsync(expectedCollections);
    }

    private void MockCache<T>()
    {
        _mockCacheProvider
            .Setup(x => x.GetOrSetFromCacheAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<T>>>(),
                It.IsAny<bool>()))
            .Returns((string cacheKey, Func<Task<T>> getDataFunc, bool useCache) =>
            {
                return getDataFunc();
            });
    }
}
