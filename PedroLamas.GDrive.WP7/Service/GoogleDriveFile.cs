using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveFile
    {
        public const string FolderMimeType = "application/vnd.google-apps.folder";

        #region Properties

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("etag")]
        public string ETag { get; set; }

        [JsonProperty("selfLink")]
        public string SelfLink { get; set; }

        [JsonProperty("alternateLink")]
        public string AlternateLink { get; set; }

        [JsonProperty("thumbnailLink")]
        public string ThumbnailLink { get; set; }

        [JsonProperty("permissionsLink")]
        public string PermissionsLink { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("labels")]
        public GoogleDriveLabels Labels { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty("modifiedByMeDate")]
        public DateTime? ModifiedByMeDate { get; set; }

        [JsonProperty("lastViewedByMeDate")]
        public DateTime? LastViewedByMeDate { get; set; }

        [JsonProperty("parents")]
        public IEnumerable<GoogleDriveParent> Parents { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("originalFilename")]
        public string OriginalFilename { get; set; }

        [JsonProperty("fileExtension")]
        public string FileExtension { get; set; }

        [JsonProperty("md5Checksum")]
        public string Md5Checksum { get; set; }

        [JsonProperty("fileSize")]
        public int? FileSize { get; set; }

        [JsonProperty("ownerNames")]
        public IEnumerable<string> OwnerNames { get; set; }

        [JsonProperty("lastModifyingUserName")]
        public string LastModifyingUserName { get; set; }

        [JsonProperty("editable")]
        public bool? Editable { get; set; }

        [JsonProperty("writersCanShare")]
        public bool? WritersCanShare { get; set; }

        #endregion

        public bool IsChildOf(GoogleDriveFile file)
        {
            if (Parents == null)
            {
                return false;
            }

            if (file == null)
            {
                return Parents
                    .Any(y => y.IsRoot);
            }
            else
            {
                var currentFolderId = file.Id;

                return Parents
                    .Any(x => x.Id == currentFolderId);
            }
        }
    }
}