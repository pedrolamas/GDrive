using System.Collections.Generic;

namespace PedroLamas.WP7.GDrive.Model
{
    public interface IFileModel
    {
        string ChildrenETag { get; set; }

        IDictionary<string, FileModel> Children { get; }

        string Id { get; }

        string Title { get; }
    }
}