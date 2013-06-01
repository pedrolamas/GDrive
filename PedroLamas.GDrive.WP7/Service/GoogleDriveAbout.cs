using System.Collections.Generic;
using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveAbout
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string ETag { get; set; }

        [JsonProperty("selfLink")]
        public string SelfLink { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quotaBytesTotal")]
        public long? QuotaBytesTotal { get; set; }

        [JsonProperty("quotaBytesUsed")]
        public long? QuotaBytesUsed { get; set; }

        [JsonProperty("quotaBytesUsedInTrash")]
        public long? QuotaBytesUsedInTrash { get; set; }

        [JsonProperty("largestChangeId")]
        public long? LargestChangeId { get; set; }

        [JsonProperty("remainingChangeIds")]
        public long? RemainingChangeIds { get; set; }

        [JsonProperty("rootFolderId")]
        public string RootFolderId { get; set; }

        [JsonProperty("domainSharingPolicy")]
        public string DomainSharingPolicy { get; set; }

        [JsonProperty("permissionId")]
        public string PermissionId { get; set; }

        [JsonProperty("importFormats")]
        public IEnumerable<GoogleDriveFormat> ImportFormats { get; set; }

        [JsonProperty("exportFormats")]
        public IEnumerable<GoogleDriveFormat> ExportFormats { get; set; }

        [JsonProperty("additionalRoleInfo")]
        public IEnumerable<GoogleDriveRoleInfo> AdditionalRoleInfo { get; set; }

        [JsonProperty("features")]
        public IEnumerable<GoogleDriveFeature> Features { get; set; }

        [JsonProperty("maxUploadSizes")]
        public IEnumerable<GoogleDriveMaxUploadSize> MaxUploadSizes { get; set; }

        [JsonProperty("isCurrentAppInstalled")]
        public bool? IsCurrentAppInstalled { get; set; }
    }
}