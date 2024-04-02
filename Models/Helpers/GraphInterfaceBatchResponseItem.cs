using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace GraphInterface.Models.Helpers;
internal class GraphInterfaceBatchResponseItem(string id)
{
    [JsonProperty("id")]
    public string Id { get; set; } = id;
    [JsonProperty("status")]
    public HttpStatusCode StatusCode { get; set; }
    [JsonProperty("headers")]
    public Dictionary<string, string> Headers { get; set; } = [];
    [JsonProperty("body")]
    public object? Body { get; set; } = null;
}