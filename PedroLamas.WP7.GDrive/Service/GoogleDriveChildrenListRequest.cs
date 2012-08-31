namespace PedroLamas.WP7.GDrive.Service
{
    public class GoogleDriveChildrenListRequest
    {
        public string FolderId { get; set; }

        public int? MaxResults { get; set; }

        public string PageToken { get; set; }

        public string Query { get; set; }

        public string Fields { get; set; }

        public string ETag { get; set; }
    }
}