using System;
using System.Text.Json.Serialization;
using GraphInterface.Services.Converters;

namespace GraphInterface.Auth;
public class GraphInterfaceAccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
    [JsonPropertyName("expires_in")]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan ExpiresIn { get; set; } = TimeSpan.Zero;
    [JsonPropertyName("ext_expires_in")]
    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public TimeSpan ExtExpiresIn { get; set; } = TimeSpan.Zero;
    [JsonPropertyName("expires_on")]
    public DateTime ExpiresOn { get; set; } = DateTime.MinValue;
    [JsonPropertyName("not_before")]
    public DateTime NotBefore { get; set; } = DateTime.MinValue;
    [JsonPropertyName("resource")]
    public string Resource { get; set; } = string.Empty;
}