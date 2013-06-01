namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveFilesUpdateRequest
    {
        public GoogleDriveFile File { get; set; }

        public string Fields { get; set; }
    }
}