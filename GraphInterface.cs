using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using GraphInterface.Models.Auth;
using GraphInterface.Models.Options;
using GraphInterface.Services;
using System.Net.Http.Headers;
using System.Text;

namespace GraphInterface
{
    public class GraphInterfaceClient
    {
        private readonly GraphInterfaceCredentials _credentials;
        private readonly GraphInterfaceOptions _options;
        private readonly string _endpoint;
        private const string TOKEN_KEY = "INTERNAL::TOKEN_CACHE_KEY";
        public GraphInterfaceClient(GraphInterfaceCredentials credentials): this(credentials, new GraphInterfaceOptions()) {}
        public GraphInterfaceClient(GraphInterfaceCredentials credentials, GraphInterfaceOptions options)
        {
            _credentials = credentials;
            _options = options;
            _endpoint = $"https://graph.microsoft.com/{options.Version}";
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

            Catch(response, responseString);

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
        public async Task<T> Unit<T>(string resource, GraphInterfaceUnitOptions options) where T : class
        {
            _options.Logger.LogDebug("Generating unit request hash key");
            string hash = GraphInterfaceRequestHasher.Hash(resource, options);

            if (options.UseCache)
            {
                AssertCacheIsNotNull();
                
                if (_options.CacheService.Has(hash))
                {
                    _options.Logger.LogDebug("Returning cached unit response");
                    return _options.CacheService.Get<T>(hash);
                }
            }

            var request = new HttpRequestMessage(options.Method, $"{_endpoint}/{resource}");
            string token = await GetAccessToken();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            foreach (var item in options.Headers)
            {
                request.Headers.Add(item.Key, item.Value);
            }

            if (options.Body != null)
            {
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(options.Body),
                    Encoding.UTF8,
                    "application/json"
                );
            }

            _options.Logger.LogDebug("Sending unit request");
            var response = await _options.HttpClient.SendAsync(request);
            string responseString = await response.Content.ReadAsStringAsync();

            Catch(response, responseString);

            T result = JsonConvert.DeserializeObject<T>(responseString);

            if (options.UseCache)
            {
                _options.Logger.LogDebug("Caching unit response");
                _options.CacheService.Set(hash, result);
            }

            _options.Logger.LogDebug("Returning unit response");
            return result;
        }
        public async Task<T> Unit<T>(string resource) where T : class
        {
            return await Unit<T>(resource, new GraphInterfaceUnitOptions());
        }
        private void Catch(HttpResponseMessage response, string responseString)
        {
            if (!response.IsSuccessStatusCode)
            {
                _options.Logger.LogError("Failed to complete request");
                string message = string.IsNullOrEmpty(responseString) ? "Unknown error" : responseString;
                
                throw new Exception($"Failed to complete request with Error {(int)response.StatusCode} - {response.StatusCode}: {message}");
            }
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