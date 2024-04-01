using System.Collections.Generic;
using Newtonsoft.Json;

namespace GraphInterface.Models.Helpers;
internal class GraphInterfaceListResponse<T> where T : class
{
    [JsonProperty("@odata.context")]
    public string Context { get; set; } = string.Empty;
    [JsonProperty("@odata.nextLink")]
    public string? NextLink { get; set; } = null;
    [JsonProperty("value")]
    public IEnumerable<T> Value { get; set; } = [];
}