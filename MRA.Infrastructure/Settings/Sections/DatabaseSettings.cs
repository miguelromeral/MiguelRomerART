namespace MRA.Infrastructure.Settings.Options;

public class DatabaseSettings
{
    public string Name { get; set; }
    public DatabaseCollectionsOptions Collections { get; set; }
    public DatabaseDrawingsOptions Drawings { get; set; }

    public class DatabaseCollectionsOptions
    {
        public string Drawings { get; set; }
        public string Inspirations { get; set; }
        public string Collections { get; set; }
        public string Experience { get; set; }
    }

    public class DatabaseDrawingsOptions
    {
        public DatabaseDrawingsTagsOptions Tags { get; set; }
    }

    public class DatabaseDrawingsTagsOptions
    {
        public IEnumerable<string> Delete { get; set; }
        public IEnumerable<DatabaseDrawingsTagsReplaceOptions> Replace { get; set; }

        public class DatabaseDrawingsTagsReplaceOptions
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
