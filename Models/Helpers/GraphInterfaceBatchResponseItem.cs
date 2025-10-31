using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace GraphInterface.Models.Helpers;
internal class GraphInterfaceBatchResponseItem(string id)
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = id;
    [JsonPropertyName("status")]
    public HttpStatusCode StatusCode { get; set; }
    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; } = [];
    [JsonPropertyName("body")]
    public object? Body { get; set; } = null;
}