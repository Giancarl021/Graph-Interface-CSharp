using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GraphInterface.Models.Helpers;
internal class GraphInterfaceBatchResponse
{
    [JsonPropertyName("responses")]
    public IEnumerable<GraphInterfaceBatchResponseItem> Responses { get; set; } = [];
    public bool IsSuccessful { get; set; } = true;
    public IEnumerable<string> RejectedIds { get; set; } = [];
}