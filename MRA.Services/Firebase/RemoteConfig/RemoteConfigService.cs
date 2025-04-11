using Microsoft.Extensions.Caching.Memory;
using MRA.DTO.Firebase.RemoteConfig;
using MRA.Infrastructure.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MRA.Services.Firebase.RemoteConfig
{
    public class RemoteConfigService : BaseCacheService, IRemoteConfigService
    {
        private readonly HttpClient _httpClient;
        private readonly string CACHE_REMOTE_CONFIG = "remote_config";
        private readonly AppConfiguration _appConfiguration;

        public RemoteConfigService(IMemoryCache cache, AppConfiguration appConfig)
            : base(cache)
        {
            _appConfiguration = appConfig;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"https://firebaseremoteconfig.googleapis.com/v1/projects/{_appConfiguration.Firebase.ProjectID}/remoteConfig")
            };
        }

        private async Task<HttpClient> GetHttpClientAsync()
        {
            var accessToken = await GoogleCredentialHelper.GetAccessTokenAsync(_appConfiguration.Firebase.CredentialsPath);

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri($"https://firebaseremoteconfig.googleapis.com/v1/projects/{_appConfiguration.Firebase.ProjectID}/remoteConfig")
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return httpClient;
        }

        public async Task<RemoteConfigResponse> GetRemoteConfig()
        {
            return await GetOrSetAsync(CACHE_REMOTE_CONFIG, async () =>
            {
                return await GetRemoteConfigInfo();
            }, 
            useCache: true, TimeSpan.FromSeconds(_appConfiguration.Cache.RefreshSeconds));
        }

        private async Task<RemoteConfigResponse> GetRemoteConfigInfo()
        {
            var httpClient = await GetHttpClientAsync();

            var response = await httpClient.GetAsync("");
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<RemoteConfigResponse>(jsonResponse);
        }

        public async Task<T> GetConfigValueAsync<T>(RemoteConfigKey<T> key)
        {
            var remoteConfig = await GetRemoteConfig();
            return remoteConfig.GetParameter(key);
        }
    }
}
