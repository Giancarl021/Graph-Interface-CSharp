using System;
using GraphInterface.Services;
using Newtonsoft.Json;

namespace GraphInterface.Models.Auth
{
    public class GraphInterfaceAccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("expires_in")]
        [JsonConverter(typeof(TimeSpanJsonConverter))]
        public TimeSpan ExpiresIn { get; set; }
        [JsonProperty("ext_expires_in")]
        [JsonConverter(typeof(TimeSpanJsonConverter))]
        public TimeSpan ExtExpiresIn { get; set; }
        [JsonProperty("expires_on")]
        public DateTime ExpiresOn { get; set; }
        [JsonProperty("not_before")]
        public DateTime NotBefore { get; set; }
        [JsonProperty("resource")]
        public string Resource { get; set; }
    }
}