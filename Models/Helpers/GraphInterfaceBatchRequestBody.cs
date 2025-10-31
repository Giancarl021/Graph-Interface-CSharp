using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GraphInterface.Models.Helpers;
internal class GraphInterfaceBatchRequestBody
{
    [JsonPropertyName("requests")]
    public IEnumerable<GraphInterfaceBatchRequestItem> Requests;
    public GraphInterfaceBatchRequestBody(IEnumerable<GraphInterfaceBatchRequestItem> requests)
    {
        Requests = requests;

        if (Requests == null)
            throw new ArgumentException("Requests is required");
    }
}