using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Throttle;

using GraphInterface.Auth;
using GraphInterface.Options;
using GraphInterface.Services;
using GraphInterface.Models.Helpers;
using Newtonsoft.Json.Linq;

namespace GraphInterface
{
    public class GraphInterfaceClient
    {
        private readonly GraphInterfaceCredentials _credentials;
        private readonly GraphInterfaceOptions _options;
        private readonly string _endpoint;
        private readonly Uri _batchEndpoint;
        private const string TOKEN_KEY = "INTERNAL::TOKEN_CACHE_KEY";
        private const int BATCH_REQUEST_SIZE = 20;
        public GraphInterfaceClient(GraphInterfaceCredentials credentials): this(credentials, new GraphInterfaceOptions()) {}
        public GraphInterfaceClient(GraphInterfaceCredentials credentials, GraphInterfaceOptions options)
        {
            _credentials = credentials;
            _options = options;
            _endpoint = $"https://graph.microsoft.com/{options.Version}";
            _batchEndpoint = new Uri($"{_endpoint}/$batch", UriKind.Absolute);
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
            if (string.IsNullOrWhiteSpace(resource))
            {
                throw new Exception("Resource cannot be null, empty or whitespace only");
            }

            string hash = options.UseCache ? GraphInterfaceRequestHasher.Hash(resource, options) : null;

            if (options.UseCache)
            {
                AssertCacheIsNotNull();
                
                if (_options.CacheService.Has(hash))
                {
                    _options.Logger.LogDebug("Returning cached unit response");
                    return _options.CacheService.Get<T>(hash);
                }
            }

            var uri = new Uri(resource, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri($"{_endpoint}/{resource}", UriKind.Absolute);
            }

            var request = new HttpRequestMessage(options.Method, uri);
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
        public async Task<IEnumerable<T>> List<T>(string resource, GraphInterfaceListOptions options) where T : class
        {
            if (string.IsNullOrWhiteSpace(resource))
            {
                throw new Exception("Resource cannot be null, empty or whitespace only");
            }

            if (options.Limit == 0)
            {
                return new List<T>();
            }

            _options.Logger.LogDebug("Generating list request hash key");
            string hash = options.UseCache ? GraphInterfaceRequestHasher.Hash(resource, options) : null;

            if (options.UseCache)
            {
                AssertCacheIsNotNull();
                
                if (_options.CacheService.Has(hash))
                {
                    _options.Logger.LogDebug("Returning cached list response");
                    return _options.CacheService.Get<IEnumerable<T>>(hash);
                }
            }

            var unitOptions = options.ToUnitOptions();
            int index = 0;
            int offset = options.Offset.GetValueOrDefault(0);

            string nextUri = resource;
            GraphInterfaceListResponse<T> response;
            Func<int, bool> hasFinished = index =>
            {
                if (options.Limit == null) return false;

                return (index - offset) == options.Limit.GetValueOrDefault();
            };

            var result = new List<T>();

            do
            {
                response = await Unit<GraphInterfaceListResponse<T>>(nextUri, unitOptions);

                if (index >= offset)
                {
                    result.AddRange(response.Value);
                }

                nextUri = response.NextLink;

                index++;
            } while (!string.IsNullOrEmpty(nextUri) && !hasFinished(index));

            if (options.UseCache)
            {
                _options.Logger.LogDebug("Caching list response");
                _options.CacheService.Set<IEnumerable<T>>(hash, result);
            }

            return result;
        }
        public async Task<IEnumerable<T>> List<T>(string resource) where T : class
        {
            return await List<T>(resource, new GraphInterfaceListOptions());
        }
        public async Task<Dictionary<string, T>> Massive<T>(string format, GraphInterfaceMassiveOptions options) where T : class
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new Exception("Format cannot be null, empty or whitespace only");
            }

            options.Assert();

            var resourcesBuilder = new GraphInterfaceMassiveResourcesBuilder(format, options.Values);
            var resources = resourcesBuilder.Build();

            int l = resources.Count();
            
            var binderList = options
                .Values
                .ToList()
                [(int)options.BinderIndex]
                .ToList();
            
            var remaining = new Dictionary<string, GraphInterfaceBatchResponse>();
            var results = new Dictionary<string, T>();
            int attempts = 0;

            do
            {
                var requests = Bind(resources);
                var packages = Pack(requests);

                var responses = await Throttler.Throttle(packages, (int)options.RequestsPerAttempt);

                var result = Unpack(responses);

                foreach (var item in result.Resolved)
                {
                    results.Add(item.Id, JObject.FromObject(item.Body).ToObject<T>());
                }

                if (resources.Count() == result.Rejected.Count()) attempts++;

                if (attempts >= options.Attempts)
                {
                    if (!options.NullifyErrors) throw new Exception("Maximum attempts reached");
                    
                    foreach (var id in result.Rejected)
                    {
                        results.Add(id, null);
                    }

                    break;
                }

                resources = result.Rejected;
            } while (resources.Count() > 0);

            return results;

            IEnumerable<GraphInterfaceBatchRequestItem> Bind(IEnumerable<string> resources)
            {
                return resources.Select((resource, index) =>
                    new GraphInterfaceBatchRequestItem
                    {
                        Url = new Uri(resource, UriKind.Relative),
                        Method = options.Method,
                        Headers = options.Headers,
                        Body = options.Body,
                        Id = binderList[index]
                    }
                );
            }

            IEnumerable<Func<Task<GraphInterfaceBatchResponse>>> Pack(IEnumerable<GraphInterfaceBatchRequestItem> requestItems)
            {
                var requests = requestItems.Where(request => request != null);

                var packages = new List<Func<Task<GraphInterfaceBatchResponse>>>();

                for (int i = 0; i < l; i += BATCH_REQUEST_SIZE)
                {
                    int s = Math.Min(BATCH_REQUEST_SIZE + i, l);
                    var request = new HttpRequestMessage(HttpMethod.Post, _batchEndpoint);

                    foreach (var item in options.BatchRequestHeaders)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }

                    var requestBlock = requests.Skip(i).Take(s);

                    request.Content = new StringContent(
                        JsonConvert.SerializeObject(new GraphInterfaceBatchRequestBody(requestBlock)),
                        Encoding.UTF8,
                        "application/json"
                    );

                    packages.Add(async () =>
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());
                        var response = await _options.HttpClient.SendAsync(request);

                        if (!response.IsSuccessStatusCode)
                        {
                            return new GraphInterfaceBatchResponse
                            {
                                IsSuccessful = false,
                                RejectedIds = requestBlock.Select(requestItem => requestItem.Id)
                            };
                        }

                        string responseString = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<GraphInterfaceBatchResponse>(responseString);

                        return data;
                    });
                }

                return packages;
            }

            GraphInterfaceBatchResult Unpack(IEnumerable<GraphInterfaceBatchResponse> responses)
            {
                var result = new GraphInterfaceBatchResult
                {
                    Resolved = new List<GraphInterfaceBatchResponseItem>(),
                    Rejected = new List<string>()
                };

                foreach (var response in responses)
                {
                    if (!response.IsSuccessful)
                    {
                        result.Rejected.AddRange(response.RejectedIds);
                        continue;
                    }

                    foreach (var item in response.Responses)
                    {
                        bool isSuccess = new HttpResponseMessage(item.StatusCode).IsSuccessStatusCode;
                        if (isSuccess)
                        {
                            result.Resolved.Add(item);
                        }
                        else
                        {
                            result.Rejected.Add(item.Id);
                        }
                    }
                }

                return result;
            }
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