﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace PedroLamas.WP7.GDrive.Service
{
    public class GoogleDriveChildrenListResponse
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
        public IEnumerable<GoogleDriveChild> Items { get; set; }
    }
}