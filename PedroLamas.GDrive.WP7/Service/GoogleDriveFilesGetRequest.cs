namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveFilesGetRequest
    {
        public string FileId { get; set; }

        public string Fields { get; set; }

        public string ETag { get; set; }
    }
}