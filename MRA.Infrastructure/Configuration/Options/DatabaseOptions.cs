namespace MRA.Infrastructure.Configuration.Options;

public class DatabaseOptions
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
