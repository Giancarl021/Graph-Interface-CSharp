using System;
using System.Collections.Generic;
using System.Net.Http;
using GraphInterface.Services.Converters;
using Newtonsoft.Json;

namespace GraphInterface.Models.Helpers
{
    internal class GraphInterfaceBatchResponse
    {
        [JsonProperty("responses")]
        public IEnumerable<GraphInterfaceBatchResponseItem> Responses { get; set; }
        public bool IsSuccessful { get; set; } = true;
        public IEnumerable<string> RejectedIds { get; set; } = null;
    }
}