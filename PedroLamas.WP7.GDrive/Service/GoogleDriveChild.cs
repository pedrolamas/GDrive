using Newtonsoft.Json;

namespace PedroLamas.WP7.GDrive.Service
{
    public class GoogleDriveChild
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("selfLink")]
        public string SelfLink { get; set; }

        [JsonProperty("childLink")]
        public string ChildLink { get; set; }
    }
}