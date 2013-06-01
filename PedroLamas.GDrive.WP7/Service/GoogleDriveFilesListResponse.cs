using System.Collections.Generic;
using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveFilesListResponse
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string ETag { get; set; }

        [JsonProperty("selfLink")]
        public string SelfLink { get; set; }

        [JsonProperty("nextPageToken")]
        public string NextPageToken { get; set; }

        [JsonProperty("nextLink")]
        public string NextLink { get; set; }

        [JsonProperty("items")]
        public IEnumerable<GoogleDriveFile> Items { get; set; }
    }
}