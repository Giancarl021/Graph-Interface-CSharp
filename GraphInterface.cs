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
using System.Xml.XPath;

namespace GraphInterface;

public class GraphInterfaceClient
{
    private readonly GraphInterfaceCredentials _credentials;
    private readonly GraphInterfaceOptions _options;
    private readonly string _endpoint;
    private readonly Uri _batchEndpoint;
    private const string TOKEN_KEY = "INTERNAL::TOKEN_CACHE_KEY";
    private const int BATCH_REQUEST_SIZE = 20;
    public GraphInterfaceClient(GraphInterfaceCredentials credentials) : this(credentials, new GraphInterfaceOptions()) { }
    public GraphInterfaceClient(GraphInterfaceCredentials credentials, GraphInterfaceOptions options)
    {
        _credentials = credentials;
        _options = options;
        _endpoint = $"https://graph.microsoft.com/{options.Version}";
        _batchEndpoint = new Uri($"{_endpoint}/$batch", UriKind.Absolute);

        if (_options.CacheService == null)
        {
            _options.CacheAccessTokenByDefault = false;
        }

        AssertHttpClientIsNotNull();
    }
    public async Task<string> GetAccessToken(GraphInterfaceAccessTokenOptions options)
    {
        if (options.UseCache)
        {
            AssertCacheIsNotNull();

            if (await _options.CacheService.Has(TOKEN_KEY))
            {
                _options.Logger.LogDebug("Returning cached access token");
                return (await _options.CacheService.Get<GraphInterfaceAccessTokenResponse>(TOKEN_KEY))!.AccessToken;
            }
        }

        if (_options.AuthenticationProvider != null)
        {
            _options.Logger.LogDebug("Retrieving access token from custom authentication provider");

            var customToken = await _options.AuthenticationProvider(_credentials);
            if (options.UseCache)
            {
                _options.Logger.LogDebug("Caching access token");
                await _options.CacheService.Set(TOKEN_KEY, customToken, customToken.ExpiresIn);
            }
            _options.Logger.LogDebug("Returning access token from custom authentication provider");
            return customToken.AccessToken;
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"https://login.microsoftonline.com/{_credentials.TenantId}/oauth2/v2.0/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _credentials.ClientId },
                { "client_secret", _credentials.ClientSecret },
                { "grant_type", "client_credentials" },
                { "scope", "https://graph.microsoft.com/.default" }
            })
        };

        _options.Logger.LogDebug("Requesting new access token");
        var response = await _options.HttpClient.SendAsync(request);
        string responseString = await response.Content.ReadAsStringAsync();

        Catch(response, responseString);

        var token = JsonConvert.DeserializeObject<GraphInterfaceAccessTokenResponse>(responseString)!;

        if (options.UseCache)
        {
            _options.Logger.LogDebug("Caching access token");
            await _options.CacheService.Set(TOKEN_KEY, token, token.ExpiresIn);
        }

        _options.Logger.LogDebug("Returning access token");
        return token.AccessToken;
    }
    public async Task<string> GetAccessToken() => await GetAccessToken(new GraphInterfaceAccessTokenOptions
    {
        UseCache = _options.CacheAccessTokenByDefault
    });
    public async Task<string> Raw(string resource) => await Raw(resource, new GraphInterfaceRawOptions());
    public async Task<string> Raw(string resource, GraphInterfaceRawOptions options)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new Exception("Resource cannot be null, empty or whitespace only");
        }

        var uri = new Uri(resource, UriKind.RelativeOrAbsolute);

        if (!uri.IsAbsoluteUri)
        {
            uri = new Uri($"{_endpoint}/{resource}", UriKind.Absolute);
        }

        var request = new HttpRequestMessage(options.Method, uri);
        string token = options.CustomAccessToken ?? await GetAccessToken();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        foreach (var item in options.Headers)
            request.Headers.Add(item.Key, item.Value);

        if (options.Body != null)
            request.Content = new StringContent(
                JsonConvert.SerializeObject(options.Body),
                Encoding.UTF8,
                "application/json"
            );

        _options.Logger.LogDebug("Sending raw request");
        var response = await _options.HttpClient.SendAsync(request);
        string responseString = await response.Content.ReadAsStringAsync();

        Catch(response, responseString);

        _options.Logger.LogDebug("Returning raw response");

        return responseString;
    }
    public async Task<T> Unit<T>(string resource, GraphInterfaceUnitOptions options) where T : class
    {
        string? hash = options.UseCache ? GraphInterfaceRequestHasher.Hash(resource, options) : null;

        if (options.UseCache)
        {
            AssertCacheIsNotNull();

            if (await _options.CacheService.Has(hash!))
            {
                _options.Logger.LogDebug("Returning cached unit response");
                return (await _options.CacheService.Get<T>(hash))!;
            }
        }

        string responseString = await Raw(resource, options.ToRawOptions());

        _options.Logger.LogDebug("Deserializing unit response");

        T result = JsonConvert.DeserializeObject<T>(responseString)!;

        if (options.UseCache)
        {
            _options.Logger.LogDebug("Caching unit response");
            await _options.CacheService.Set(hash!, result);
        }

        return result;
    }
    public async Task<T> Unit<T>(string resource) where T : class => await Unit<T>(resource, new GraphInterfaceUnitOptions());
    public async Task<IEnumerable<T>> List<T>(string resource, GraphInterfaceListOptions options) where T : class
    {
        if (string.IsNullOrWhiteSpace(resource))
            throw new Exception("Resource cannot be null, empty or whitespace only");

        if (options.Limit == 0)
            return [];

        _options.Logger.LogDebug("Generating list request hash key");
        string? hash = options.UseCache ? GraphInterfaceRequestHasher.Hash(resource, options) : null;

        if (options.UseCache)
        {
            AssertCacheIsNotNull();

            if (await _options.CacheService.Has(hash!))
            {
                _options.Logger.LogDebug("Returning cached list response");
                return (await _options.CacheService.Get<IEnumerable<T>>(hash))!;
            }
        }

        var unitOptions = options.ToUnitOptions();
        int index = 0;
        int limit = options.Limit.GetValueOrDefault(0);
        int offset = options.Offset.GetValueOrDefault(0);

        string? nextUri = resource;
        GraphInterfaceListResponse<T> response;

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
            await _options.CacheService.Set<IEnumerable<T>>(hash, result);
        }

        return result;

        bool hasFinished(int index)
        {
            if (options.Limit == null) return false;

            return (index - offset) == limit;
        }
    }
    public async Task<IEnumerable<T>> List<T>(string resource) where T : class => await List<T>(resource, new GraphInterfaceListOptions());
    public async Task<Dictionary<string, T?>> Massive<T>(string format, GraphInterfaceMassiveOptions options) where T : class
    {
        if (string.IsNullOrWhiteSpace(format))
            throw new Exception("Format cannot be null, empty or whitespace only");

        options.Assert();

        _options.Logger.LogDebug("Generating massive request hash key");
        string? hash = options.UseCache ? GraphInterfaceRequestHasher.Hash(format, options) : null;

        if (options.UseCache)
        {
            AssertCacheIsNotNull();

            if (await _options.CacheService.Has(hash))
            {
                _options.Logger.LogDebug("Returning cached massive response");
                return (await _options.CacheService.Get<Dictionary<string, T>>(hash))!;
            }
        }


        _options.Logger.LogDebug("Generating individual urls");
        var resourcesBuilder = new GraphInterfaceMassiveResourcesBuilder(format, options.Values);
        var resources = resourcesBuilder.Build();

        int l = resources.Count();
        var urls = new Dictionary<string, Uri>();

        var binderList = options
            .Values
            .ToList()
            [(int)options.BinderIndex]
            .ToList();

        var results = new Dictionary<string, T?>();
        int attempts = 0;

        _options.Logger.LogDebug("Generating individual requests");

        var requests = resources.Select((resource, index) =>
            new GraphInterfaceBatchRequestItem(binderList[index], new Uri(resource, UriKind.Relative), options.Method)
            {
                Headers = options.Headers,
                Body = options.Body
            }
        );

        requests
            .ToList()
            .ForEach(request =>
            {
                urls.Add(request.Id, request.Url);
            });

        do
        {
            _options.Logger.LogDebug("Packaging requests into Graph batch requests");
            var packages = Pack(requests);

            _options.Logger.LogDebug("Sending batch requests");
            var responses = await Throttler.Throttle(packages, (int)options.RequestsPerAttempt);

            _options.Logger.LogDebug("Resolving batch responses");
            var result = Unpack(responses);

            foreach (var item in result.Resolved)
            {
                results.Add(item.Id, JObject.FromObject(item.Body!).ToObject<T>()!);
            }

            if (resources.Count == result.Rejected.Count)
            {
                _options.Logger.LogDebug("All requests were rejected");
                attempts++;
            }

            if (attempts >= options.Attempts)
            {
                if (!options.NullifyErrors) throw new Exception("Maximum attempts reached");

                _options.Logger.LogDebug("Maximum attempts reached, nullifying errors");
                foreach (var id in result.Rejected)
                    results.Add(id, null);

                break;
            }

            resources = result.Rejected;
            l = resources.Count();

            _options.Logger.LogDebug("Generating individual requests");
            requests = Rebind(resources);
        } while (resources.Count() > 0);

        if (options.UseCache)
        {
            _options.Logger.LogDebug("Caching massive response");
            await _options.CacheService.Set(hash, results);
        }

        return results;

        IEnumerable<GraphInterfaceBatchRequestItem> Rebind(IEnumerable<string> ids) => ids.Select(id =>
            new GraphInterfaceBatchRequestItem(id, urls[id], options.Method)
            {
                Headers = options.Headers,
                Body = options.Body
            });

        IEnumerable<Func<Task<GraphInterfaceBatchResponse>>> Pack(IEnumerable<GraphInterfaceBatchRequestItem> requestItems)
        {
            var requests = requestItems.Where(request => request != null);

            var packages = new List<Func<Task<GraphInterfaceBatchResponse>>>();

            for (int i = 0; i < l; i += BATCH_REQUEST_SIZE)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _batchEndpoint);

                foreach (var item in options.BatchRequestHeaders)
                {
                    request.Headers.Add(item.Key, item.Value);
                }

                var skipped = requests.Skip(i);
                var requestBlock = skipped.Take(Math.Min(BATCH_REQUEST_SIZE, skipped.Count()));

                request.Content = new StringContent(
                    JsonConvert.SerializeObject(new GraphInterfaceBatchRequestBody(requestBlock)),
                    Encoding.UTF8,
                    "application/json"
                );

                packages.Add(async () =>
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.CustomAccessToken ?? await GetAccessToken());
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

                    if (data == null)
                        return new GraphInterfaceBatchResponse
                        {
                            IsSuccessful = false,
                            RejectedIds = requestBlock.Select(requestItem => requestItem.Id)
                        };

                    return data;
                });
            }

            return packages;
        }

        GraphInterfaceBatchResult Unpack(IEnumerable<GraphInterfaceBatchResponse> responses)
        {
            var result = new GraphInterfaceBatchResult();

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
    public IAsyncEnumerable<IEnumerable<T>> CreateListGenerator<T>(string resource) where T : class => CreateListGenerator<T>(resource, new GraphInterfaceListGeneratorOptions());
    public async IAsyncEnumerable<IEnumerable<T>> CreateListGenerator<T>(string resource, GraphInterfaceListGeneratorOptions options) where T : class
    {
        if (string.IsNullOrWhiteSpace(resource))
            throw new Exception("Resource cannot be null, empty or whitespace only");

        if (options.Limit == 0)
            yield break;

        var unitOptions = options.ToUnitOptions();
        int index = 0;
        int limit = options.Limit.GetValueOrDefault(0);
        int offset = options.Offset.GetValueOrDefault(0);

        string? nextUri = resource;
        GraphInterfaceListResponse<T> response;

        do
        {
            response = await Unit<GraphInterfaceListResponse<T>>(nextUri, unitOptions);

            if (index >= offset) yield return response.Value;

            nextUri = response.NextLink;

            index++;
        } while (!string.IsNullOrEmpty(nextUri) && !hasFinished(index));

        bool hasFinished(int index)
        {
            if (options.Limit == null) return false;

            return (index - offset) == limit;
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
