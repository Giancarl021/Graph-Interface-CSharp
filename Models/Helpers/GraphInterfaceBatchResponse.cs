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
    }
}