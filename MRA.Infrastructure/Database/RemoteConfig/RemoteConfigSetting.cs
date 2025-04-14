namespace MRA.Infrastructure.Database.RemoteConfig;

public class RemoteConfigSetting<T>
{
    public RemoteConfigKey Key { get; private set; }
    public T DefaultValue { get; private set; }

    public RemoteConfigSetting(RemoteConfigKey key, T defaultValue)
    {
        Key = key;
        DefaultValue = defaultValue;
    }
}
