namespace MRA.Infrastructure.Database.RemoteConfig;

public interface IRemoteConfigDatabase
{
    T GetValue<T>(RemoteConfigSetting<T> remoteConfig);
}
