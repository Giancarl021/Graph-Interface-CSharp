using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json.Serialization;
using GraphInterface.Services.Converters;

namespace GraphInterface.Models.Helpers;
internal class GraphInterfaceBatchRequestItem(string id, Uri url, HttpMethod method)
{
    [JsonPropertyName("headers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Headers { get; set; }
    [JsonPropertyName("url")]
    public Uri Url { get; set; } = url;
    [JsonPropertyName("method")]
    [JsonConverter(typeof(HttpMethodJsonConverter))]
    public HttpMethod Method { get; set; } = method;
    [JsonPropertyName("id")]
    public string Id { get; set; } = id;
    [JsonPropertyName("body")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Body { get; set; }
}