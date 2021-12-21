using System;
using Newtonsoft.Json;

namespace GraphInterface.Models
{
    public class GraphInterfaceAccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public TimeSpan ExpiresIn { get; set; }
    }
}