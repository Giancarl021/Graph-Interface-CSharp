using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GraphInterface.Models;
using GraphInterface.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphInterface
{
    public class GraphInterfaceClient
    {
        private readonly GraphInterfaceCredentials _credentials;
        private readonly GraphInterfaceOptions _options;
        public GraphInterfaceClient(GraphInterfaceCredentials credentials)
        {
            _credentials = credentials;
            _options = GraphInterfaceOptions.GetDefault();
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
                if (_options.CacheService.Has())
                    return _options.CacheService.Get().AccessToken;
            }

            if (_options.AuthenticationProvider != null)
            {
                var customToken = await _options.AuthenticationProvider(_credentials);
                if (options.UseCache)
                {
                    _options.CacheService.Set(customToken, customToken.ExpiresIn);
                }

                return customToken.AccessToken;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://login.microsoftonline.com/{_credentials.TenantId}/oauth2/v2.0/token");

            request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _credentials.ClientId },
                { "client_secret", _credentials.ClientSecret },
                { "grant_type", "client_credentials" },
                { "scope", "https://graph.microsoft.com/.default" }
            });

            var response = await _options.HttpClient.SendAsync(request);
            var content = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Failed to get access token: " +
                    JsonConvert.SerializeObject(content["error_description"] ?? content["error"] ?? "\"Unknown error\"")
                );
            }

            var token = JObject.FromObject(content).ToObject(typeof(GraphInterfaceAccessTokenResponse));

            if (options.UseCache)
            {
                _options.CacheService.Set(token, token.ExpiresIn);
            }

            return token.AccessToken;
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