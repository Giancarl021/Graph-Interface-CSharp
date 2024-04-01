using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GraphInterface.Models.Helpers;
internal class GraphInterfaceBatchRequestBody
{
    [JsonProperty("requests")]
    public IEnumerable<GraphInterfaceBatchRequestItem> Requests;
    public GraphInterfaceBatchRequestBody(IEnumerable<GraphInterfaceBatchRequestItem> requests)
    {
        Requests = requests;

        if (Requests == null)
            throw new ArgumentException("Requests is required");
    }
}