using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace PedroLamas.WP7.GDrive.Model
{
    public class FileModelBase : ObservableObject, IFileModel
    {
        private string _id;

        #region Properties

        public string ChildrenETag { get; set; }

        public IDictionary<string, FileModel> Children { get; private set; }

        public virtual string Title
        {
            get
            {
                return "/";
            }
        }

        public virtual string Id
        {
            get
            {
                return _id;
            }
        }

        #endregion

        public FileModelBase(string id)
        {
            _id = id;

            Children = new Dictionary<string, FileModel>();
        }
    }
}