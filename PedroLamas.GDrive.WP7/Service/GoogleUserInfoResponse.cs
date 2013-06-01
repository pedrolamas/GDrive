using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleUserInfoResponse
    {
        [JsonProperty("email")]
        public string EMail { get; set; }

        [JsonProperty("verified_email")]
        public bool VerifiedEMail { get; set; }
    }
}