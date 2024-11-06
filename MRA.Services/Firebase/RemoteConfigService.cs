using Microsoft.Extensions.Caching.Memory;
using MRA.DTO.Firebase.Models;
using MRA.DTO.Firebase.RemoteConfig;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace MRA.Services.Firebase
{
    public class RemoteConfigService : BaseCacheService
    {
        private readonly int _secondsCache;
        private readonly HttpClient _httpClient;
        private readonly string _firebaseProjectId;
        private readonly string _serviceAccountPath;
        private readonly string CACHE_REMOTE_CONFIG = "remote_config";

        public RemoteConfigService(IMemoryCache cache, string firebaseProjectId, string serviceAccountPath, int secondsCache)
            : base(cache)
        {
            _firebaseProjectId = firebaseProjectId;
            _serviceAccountPath = serviceAccountPath;
            _secondsCache = secondsCache;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"https://firebaseremoteconfig.googleapis.com/v1/projects/{_firebaseProjectId}/remoteConfig")
            };
        }

        private async Task<HttpClient> GetHttpClientAsync()
        {
            var accessToken = await GoogleCredentialHelper.GetAccessTokenAsync(_serviceAccountPath);

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri($"https://firebaseremoteconfig.googleapis.com/v1/projects/{_firebaseProjectId}/remoteConfig")
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return httpClient;
        }

        public async Task<RemoteConfigResponse> GetRemoteConfig()
        {
            return await GetOrSetAsync(CACHE_REMOTE_CONFIG, async () =>
            {
                return await GetRemoteConfigInfo();
            }, TimeSpan.FromSeconds(_secondsCache));
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
