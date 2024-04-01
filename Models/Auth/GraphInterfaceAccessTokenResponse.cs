using System;
using GraphInterface.Services.Converters;
using Newtonsoft.Json;

namespace GraphInterface.Auth;
public class GraphInterfaceAccessTokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
    [JsonProperty("expires_in")]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan ExpiresIn { get; set; } = TimeSpan.Zero;
    [JsonProperty("ext_expires_in")]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan ExtExpiresIn { get; set; } = TimeSpan.Zero;
    [JsonProperty("expires_on")]
    public DateTime ExpiresOn { get; set; } = DateTime.MinValue;
    [JsonProperty("not_before")]
    public DateTime NotBefore { get; set; } = DateTime.MinValue;
    [JsonProperty("resource")]
    public string Resource { get; set; } = string.Empty;
}