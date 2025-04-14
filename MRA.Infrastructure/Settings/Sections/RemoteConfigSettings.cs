namespace MRA.Infrastructure.Settings.Options;

public class RemoteConfigSettings
{
    public string ConnectionString { get; set; }
    public RemoteConfigDefaultValuesOptions DefaultValues { get; set; }

    public class RemoteConfigDefaultValuesOptions
    {
        public RemoteConfigDefaultValuesPopularityOptions Popularity { get; set; }

        public class RemoteConfigDefaultValuesPopularityOptions
        {
            public double Critic { get; set; }
            public double Date { get; set; }
            public double Favorite { get; set; }
            public int Months { get; set; }
            public double Popular { get; set; }
        }
    }
}
