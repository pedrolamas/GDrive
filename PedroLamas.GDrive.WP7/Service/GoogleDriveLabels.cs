using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveLabels
    {
        [JsonProperty("hidden")]
        public bool? Hidden { get; set; }

        [JsonProperty("restricted")]
        public bool? Restricted { get; set; }

        [JsonProperty("starred")]
        public bool? Starred { get; set; }

        [JsonProperty("trashed")]
        public bool? Trashed { get; set; }

        [JsonProperty("viewed")]
        public bool? Viewed { get; set; }
    }
}