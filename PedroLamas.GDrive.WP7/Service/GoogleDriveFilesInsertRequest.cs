using System.IO;

namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveFilesInsertRequest
    {
        public bool? Convert { get; set; }

        public bool? Ocr { get; set; }

        public string OcrLanguage { get; set; }

        public bool? Pinned { get; set; }

        public string SourceLanguage { get; set; }

        public string TargetLanguage { get; set; }

        public string TimedTextLanguage { get; set; }

        public string TimedTextTrackName { get; set; }

        public string Fields { get; set; }

        public string Filename { get; set; }

        public byte[] FileContent { get; set; }

        public string FolderId { get; set; }

        public string ContentType
        {
            get
            {
                if (FileContent == null)
                {
                    return GoogleDriveFile.FolderMimeType;
                }

                var filenameExtension = Path.GetExtension(Filename).ToLowerInvariant();

                var contentType = "image/jpeg";

                if (!string.IsNullOrEmpty(filenameExtension))
                {
                    switch (filenameExtension)
                    {
                        case ".bmp":
                            contentType = "image/bmp";
                            break;
                        case ".gif":
                            contentType = "image/gif";
                            break;
                        case ".png":
                            contentType = "image/png";
                            break;
                        case ".tif":
                        case ".tiff":
                            contentType = "image/tiff";
                            break;
                    }
                }

                return contentType;
            }
        }
    }
}