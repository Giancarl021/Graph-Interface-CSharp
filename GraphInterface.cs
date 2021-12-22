using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using GraphInterface.Models.Auth;
using GraphInterface.Models.Options;

namespace GraphInterface
{
    public class GraphInterfaceClient
    {
        private readonly GraphInterfaceCredentials _credentials;
        private readonly GraphInterfaceOptions _options;
        private const string TOKEN_KEY = "INTERNAL::TOKEN_CACHE_KEY";
        public GraphInterfaceClient(GraphInterfaceCredentials credentials)
        {
            _credentials = credentials;
            _options = new GraphInterfaceOptions();
            AssertHttpClientIsNotNull();
        }
        public GraphInterfaceClient(GraphInterfaceCredentials credentials, GraphInterfaceOptions options)
        {
            _credentials = credentials;
            _options = options;
            AssertHttpClientIsNotNull();
        }
        public async Task<string> GetAccessToken(GraphInterfaceAccessTokenOptions options)
        {
            if (options.UseCache)
            {
                AssertCacheIsNotNull();
                
                if (_options.CacheService.Has(TOKEN_KEY))
                {
                    _options.Logger.LogDebug("Returning cached access token");
                    return _options.CacheService.Get<GraphInterfaceAccessTokenResponse>(TOKEN_KEY).AccessToken;
                }
            }

            if (_options.AuthenticationProvider != null)
            {
                _options.Logger.LogDebug("Retrieving access token from custom authentication provider");

                var customToken = await _options.AuthenticationProvider(_credentials);
                if (options.UseCache)
                {
                    _options.Logger.LogDebug("Caching access token");
                    _options.CacheService.Set(TOKEN_KEY, customToken, customToken.ExpiresIn);
                }
                _options.Logger.LogDebug("Returning access token from custom authentication provider");
                return customToken.AccessToken;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://login.microsoftonline.com/{_credentials.TenantId}/oauth2/v2.0/token");

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _credentials.ClientId },
                { "client_secret", _credentials.ClientSecret },
                { "grant_type", "client_credentials" },
                { "scope", "https://graph.microsoft.com/.default" }
            });

            _options.Logger.LogDebug("Requesting new access token");
            var response = await _options.HttpClient.SendAsync(request);
            string responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _options.Logger.LogError("Failed to retrieve access token");
                dynamic errorContent = string.IsNullOrEmpty(responseString) ? null : JsonConvert.DeserializeObject<dynamic>(responseString);
                string message = (
                        errorContent?["error_description"]?.Value ??
                        errorContent?["error"]?.Value
                    ) ?? "Unknown error";
                
                throw new Exception($"Failed to get access token with Error {(int)response.StatusCode} - {response.StatusCode}: {message}");
            }

            var token = JsonConvert.DeserializeObject<GraphInterfaceAccessTokenResponse>(responseString);

            if (options.UseCache)
            {
                _options.Logger.LogDebug("Caching access token");
                _options.CacheService.Set(TOKEN_KEY, token, token.ExpiresIn);
            }

            _options.Logger.LogDebug("Returning access token");
            return token.AccessToken;
        }
        public async Task<string> GetAccessToken()
        {
            return await GetAccessToken(new GraphInterfaceAccessTokenOptions());
        }
        public async Task<T> Unit<T>(string resource, GraphInterfaceUnitOptions options)
        {
            if (options.UseCache)
            {

            }
            string token = await GetAccessToken();

            throw new NotImplementedException();
        }
        private void AssertHttpClientIsNotNull()
        {
            if (_options.HttpClient == null) throw new Exception("HttpClient is null");
        }
        private void AssertCacheIsNotNull()
        {
            if (_options.CacheService == null) throw new Exception("CacheService is null");
        }
    }
}