using System;
using System.Globalization;
using System.Windows.Data;
using Cimbalino.Phone.Toolkit.Extensions;
using GalaSoft.MvvmLight;
using PedroLamas.GDrive.Helpers;
using PedroLamas.GDrive.Service;

namespace PedroLamas.GDrive.ViewModel
{
    public class GoogleFileViewModel : ViewModelBase
    {
        private static readonly IValueConverter _dateTimeConverter = new Microsoft.Phone.Controls.ListViewDateTimeConverter();

        private GoogleDriveFile _fileModel;

        #region Properties

        public GoogleDriveFile FileModel
        {
            get
            {
                return _fileModel;
            }
            set
            {
                if (_fileModel != value)
                {
                    _fileModel = value;

                    RaisePropertyChanged(() => FileModel);
                    RaisePropertyChanged(() => Title);
                    RaisePropertyChanged(() => Description);
                    RaisePropertyChanged(() => Starred);
                    RaisePropertyChanged(() => Icon);
                    RaisePropertyChanged(() => ThumbnailLink);
                }
            }
        }

        public string Id
        {
            get
            {
                return _fileModel.Id;
            }
        }

        public bool IsFolder
        {
            get
            {
                return string.Equals(_fileModel.MimeType, GoogleDriveFile.FolderMimeType, StringComparison.OrdinalIgnoreCase);
            }
        }

        public string Title
        {
            get
            {
                return _fileModel.Title;
            }
        }

        public string Description
        {
            get
            {
                if (_fileModel == null)
                    return " ";

                var dateString = _dateTimeConverter.Convert(_fileModel.ModifiedDate.Value, typeof(string), null, CultureInfo.InvariantCulture);

                if (_fileModel.FileSize.HasValue)
                {
                    return string.Format("Modified: {0}  Size: {1}", dateString, _fileModel.FileSize.Value.FormatAsSize());
                }

                return string.Format("Modified: {0}", dateString);
            }
        }

        public bool Starred
        {
            get
            {
                if (_fileModel.Labels == null || !_fileModel.Labels.Starred.HasValue)
                    return false;

                return _fileModel.Labels.Starred.Value;
            }
        }

        public string Icon
        {
            get
            {
                if (_fileModel == null)
                    return "/images/ic_type_file.png";

                if (IsFolder)
                {
                    return "/images/ic_type_folder.png";
                }

                switch (_fileModel.MimeType)
                {
                    case "application/vnd.google-apps.document":
                    case "application/rtf":
                    case "application/vnd.oasis.opendocument.text":
                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    case "text/html":
                        return "/images/ic_type_doc.png";

                    case "application/vnd.google-apps.spreadsheet":
                    case "text/tab-separated-values":
                    case "text/csv":
                    case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    case "application/x-vnd.oasis.opendocument.spreadsheet":
                        return "/images/ic_type_sheet.png";

                    case "vnd.google-apps.presentation":
                    case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                        return "/images/ic_type_presentation.png";

                    case "application/vnd.google-apps.drawing":
                        return "/images/ic_type_drawing.png";

                    case "application/vnd.google-apps.form":
                        return "/images/ic_type_form.png";

                    case "image/bmp":
                    case "image/gif":
                    case "image/jpeg":
                    case "image/png":
                    case "application/x-msmetafile":
                        return "/images/ic_type_image.png";

                    case "application/msword":
                        return "/images/ic_type_word.png";

                    case "application/vnd.ms-excel":
                        return "/images/ic_type_excel.png";

                    case "application/vnd.ms-powerpoint":
                        return "/images/ic_type_powerpoint.png";

                    case "application/pdf":
                        return "/images/ic_type_pdf.png";

                    default:
                        return "/images/ic_type_file.png";
                }
            }
        }

        public string ThumbnailLink
        {
            get
            {
                return _fileModel.ThumbnailLink;
            }
        }

        #endregion

        public GoogleFileViewModel(GoogleDriveFile fileModel)
        {
            FileModel = fileModel;
        }
    }
}