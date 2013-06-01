namespace PedroLamas.GDrive.Service
{
    public class GoogleDriveFilesListRequest
    {
        public int? MaxResults { get; set; }

        public string PageToken { get; set; }

        public string Projection { get; set; }

        public string Query { get; set; }

        public string Fields { get; set; }

        public string ETag { get; set; }
    }
}