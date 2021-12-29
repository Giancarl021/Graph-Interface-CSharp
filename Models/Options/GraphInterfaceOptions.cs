using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using GraphInterface.Auth;
using GraphInterface.Interfaces;
using GraphInterface.Services;

namespace GraphInterface.Options
{
    public class GraphInterfaceOptions
    {
        public string Version { get; set; } = "v1.0";
        public Func<GraphInterfaceCredentials, Task<GraphInterfaceAccessTokenResponse>> AuthenticationProvider { get; set; } = null;
        public ILogger Logger { get; set; } = NullLogger.Instance;
        public HttpClient HttpClient { get; set; } = new HttpClient();
        public IGraphInterfaceCacheService CacheService { get; set; } = new GraphInterfaceMemoryCacheService();
        public bool CacheAccessTokenByDefault { get; set; } = true;
    }
}