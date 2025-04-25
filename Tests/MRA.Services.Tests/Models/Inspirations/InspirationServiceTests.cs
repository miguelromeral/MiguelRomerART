using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Settings.Options;
using MRA.Infrastructure.Settings;
using MRA.Services.Models.Inspirations;
using Moq;
using MRA.Infrastructure.Database.Documents.MongoDb;
using static MRA.Infrastructure.Settings.Options.DatabaseSettings;
using MRA.Services.Tests.Models.Base;
using MRA.DTO.Enums.Inspirations;

namespace MRA.Services.Tests.Models.Inspirations;

public class InspirationServiceTests : DocumentModelServiceTestsBase
{
    private readonly InspirationService _service;
    protected override string COLLECTION_NAME { get => "inspirations"; }

    public InspirationServiceTests() : base()
    {
        var appSettings = new AppSettings
        {
            Database = new DatabaseSettings
            {
                Collections = new DatabaseCollectionsOptions
                {
                    Inspirations = COLLECTION_NAME
                }
            }
        };

        _service = new InspirationService(appSettings, _mockDb.Object);
    }

    [Fact]
    public async Task GetAllInspirationsAsync_Ok()
    {
        var inspirationDocuments = new List<IInspirationDocument>
        {
            new InspirationMongoDocument
            {
                Id = "1",
                Name = "Inspiration 1",
                Instagram = "@inspiration1",
                Twitter = "@inspire1",
                Type = (int) InspirationTypes.Models,
                YouTube = "Channel1",
                Twitch = "Twitch1",
                Pinterest = "Pinterest1"
            },
            new InspirationMongoDocument
            {
                Id = "2",
                Name = "Inspiration 2",
                Instagram = "@inspiration2",
                Twitter = "@inspire2",
                Type = (int) InspirationTypes.Amateurs,
                YouTube = "Channel2",
                Twitch = "Twitch2",
                Pinterest = "Pinterest2"
            }
        };

        MockGetAllDocuments(inspirationDocuments);

        var result = await _service.GetAllInspirationsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        var first = result.First();
        Assert.Equal("Inspiration 1", first.Name);
        Assert.Equal("@inspiration1", first.Instagram);
    }
}
