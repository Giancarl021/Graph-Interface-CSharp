using System;
using Microsoft.Extensions.Logging;
using GraphInterface.Interfaces;
using GraphInterface.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace GraphInterface.Models
{
    public class GraphInterfaceOptions
    {
        public string Version { get; set; } = "v1.0";
        public Func<GraphInterfaceCredentials, Task<GraphInterfaceAccessTokenResponse>> AuthenticationProvider { get; set; } = null;
        public ILogger Logger { get; set; } = null;
        public HttpClient HttpClient { get; set; }
        public IGraphInterfaceCacheService<GraphInterfaceAccessTokenResponse> CacheService { get; set; }

        public static GraphInterfaceOptions GetDefault()
        {
            return new GraphInterfaceOptions
            {
                Logger = null,
                AuthenticationProvider = null,
                HttpClient = new HttpClient(),
                CacheService = new GraphInterfaceMemoryCacheService<GraphInterfaceAccessTokenResponse>()
            };
        }
    }
}