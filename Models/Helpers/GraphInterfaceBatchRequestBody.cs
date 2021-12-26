using System;
using System.Collections.Generic;
using System.Net.Http;
using GraphInterface.Services.Converters;
using Newtonsoft.Json;

namespace GraphInterface.Models.Helpers
{
    internal class GraphInterfaceBatchRequestBody
    {
        [JsonProperty("requests")]
        public IEnumerable<GraphInterfaceBatchRequestItem> Requests;
        public GraphInterfaceBatchRequestBody(IEnumerable<GraphInterfaceBatchRequestItem> requests)
        {
            Requests = requests;
        }
    }
}