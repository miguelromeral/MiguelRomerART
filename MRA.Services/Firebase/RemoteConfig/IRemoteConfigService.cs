using MRA.DTO.Firebase.RemoteConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.RemoteConfig;

public interface IRemoteConfigService
{
    Task<RemoteConfigResponse> GetRemoteConfig();

    Task<T> GetConfigValueAsync<T>(RemoteConfigKey<T> key);
}
