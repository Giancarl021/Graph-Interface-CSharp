using System;
using Microsoft.Extensions.Logging;
using GraphInterface.Interfaces;

namespace GraphInterface.Models
{
    public class GraphInterfaceOptions
    {
        public string Version { get; set; } = "v1.0";
        public Func<GraphInterfaceCredentials, GraphInterfaceAccessTokenResponse> AuthenticationProvider { get; set; }
        public ILogger Logger { get; set; }
        public IGraphInterfaceCacheService<GraphInterfaceAccessTokenResponse> CacheService { get; set; }
    }
}