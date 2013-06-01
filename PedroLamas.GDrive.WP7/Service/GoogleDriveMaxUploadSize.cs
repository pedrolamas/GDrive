using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveMaxUploadSize
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("size")]
        public long? Size { get; set; }
    }
}