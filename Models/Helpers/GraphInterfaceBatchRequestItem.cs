using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using GraphInterface.Services.Converters;
using Newtonsoft.Json;

namespace GraphInterface.Models.Helpers;
internal class GraphInterfaceBatchRequestItem(string id, Uri url, HttpMethod method)
{
    [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, string>? Headers { get; set; }
    [JsonProperty("url")]
    public Uri Url { get; set; } = url;
    [JsonProperty("method")]
    [JsonConverter(typeof(HttpMethodJsonConverter))]
    public HttpMethod Method { get; set; } = method;
    [JsonProperty("id")]
    public string Id { get; set; } = id;
    [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
    public object? Body { get; set; }
}