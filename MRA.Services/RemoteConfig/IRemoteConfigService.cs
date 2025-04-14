namespace MRA.Services.RemoteConfig;

public interface IRemoteConfigService
{
    double GetPopularityDate();
    int GetPopularityMonths();
    double GetPopularityCritic();
    double GetPopularityPopular();
    double GetPopularityFavorite();
}
