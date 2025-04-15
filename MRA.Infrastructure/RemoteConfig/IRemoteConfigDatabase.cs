namespace MRA.Infrastructure.RemoteConfig;

public interface IRemoteConfigDatabase
{
    T GetValue<T>(RemoteConfigSetting<T> remoteConfig);
}
