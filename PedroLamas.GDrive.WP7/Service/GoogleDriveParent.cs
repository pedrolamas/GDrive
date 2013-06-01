using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveParent
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("selfLink")]
        public string SelfLink { get; set; }

        [JsonProperty("parentLink")]
        public string ParentLink { get; set; }

        [JsonProperty("isRoot")]
        public bool IsRoot { get; set; }
    }
}