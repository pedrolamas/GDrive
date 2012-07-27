using System;
using Newtonsoft.Json;

namespace PedroLamas.WP7.GDrive.Service
{
    public class GoogleAuthToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires")]
        public DateTime ExpiresDateTime { get; set; }
    }
}