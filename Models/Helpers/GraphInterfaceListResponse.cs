using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GraphInterface.Models.Helpers;
internal class GraphInterfaceListResponse<T> where T : class
{
    [JsonPropertyName("@odata.context")]
    public string Context { get; set; } = string.Empty;
    [JsonPropertyName("@odata.nextLink")]
    public string? NextLink { get; set; } = null;
    [JsonPropertyName("value")]
    public IEnumerable<T> Value { get; set; } = [];
}