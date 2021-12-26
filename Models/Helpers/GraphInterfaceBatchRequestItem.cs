using System;
using System.Collections.Generic;
using System.Net.Http;
using GraphInterface.Services.Converters;
using Newtonsoft.Json;

namespace GraphInterface.Models.Helpers
{
    internal class GraphInterfaceBatchRequestItem
    {
        [JsonProperty("headers", NullValueHandling=NullValueHandling.Ignore)]
        public Dictionary<string, string> Headers { get; set; }
        [JsonProperty("url")]
        public Uri Url { get; set; }
        [JsonProperty("method")]
        [JsonConverter(typeof(HttpMethodJsonConverter))]
        public HttpMethod Method { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("body", NullValueHandling=NullValueHandling.Ignore)]
        public object Body { get; set; }
    }
}