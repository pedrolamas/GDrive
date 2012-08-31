using Newtonsoft.Json;

namespace PedroLamas.WP7.GDrive.Service
{
    public class GoogleUserInfoResponse
    {
        [JsonProperty("email")]
        public string EMail { get; set; }

        [JsonProperty("verified_email")]
        public bool VerifiedEMail { get; set; }
    }
}