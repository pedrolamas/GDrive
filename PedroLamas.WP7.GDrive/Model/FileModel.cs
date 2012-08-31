using PedroLamas.WP7.GDrive.Service;

namespace PedroLamas.WP7.GDrive.Model
{
    public class FileModel : FileModelBase, IFileModel
    {
        private GoogleDriveFile _file;

        #region Properties

        public GoogleDriveChild Child { get; private set; }

        public GoogleDriveFile File
        {
            get
            {
                return _file;
            }
            set
            {
                if (_file == value)
                    return;

                _file = value;

                RaisePropertyChanged(() => File);
                RaisePropertyChanged(() => Title);
            }
        }

        public override string Title
        {
            get
            {
                return File != null ? File.Title : Child.Id;
            }
        }

        #endregion

        public FileModel(GoogleDriveChild child)
            : base(child.Id)
        {
            Child = child;
        }
    }
}