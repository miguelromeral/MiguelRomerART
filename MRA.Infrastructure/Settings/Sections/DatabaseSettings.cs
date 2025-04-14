namespace MRA.Infrastructure.Settings.Options;

public class DatabaseSettings
{
    public string Name { get; set; }
    public DatabaseCollectionsOptions Collections { get; set; }

    public class DatabaseCollectionsOptions
    {
        public string Drawings { get; set; }
        public string Inspirations { get; set; }
        public string Collections { get; set; }
        public string Experience { get; set; }
    }
}
