using System.Collections.Generic;
using System.Net.Http;

namespace GraphInterface.Models.Abstract;
public abstract class GraphInterfaceRequestOptions
{
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public Dictionary<string, string> Headers { get; set; } = [];
    public object? Body { get; set; } = null;
    public string? CustomAccessToken = null;
}