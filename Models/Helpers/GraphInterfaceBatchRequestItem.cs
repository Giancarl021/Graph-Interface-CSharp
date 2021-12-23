using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace GraphInterface.Models.Helpers
{
    internal class GraphInterfaceBatchRequestItem
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }
        [JsonProperty("method")]
        public HttpMethod Method { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("body")]
        public object Body { get; set; }
    }
}