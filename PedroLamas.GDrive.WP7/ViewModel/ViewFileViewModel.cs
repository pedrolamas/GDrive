using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PedroLamas.GDrive.Helpers;
using PedroLamas.GDrive.Model;
using PedroLamas.GDrive.Service;

namespace PedroLamas.GDrive.ViewModel
{
    public class ViewFileViewModel : ViewModelBase
    {
        private readonly IMainModel _mainModel;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ISystemTrayService _systemTrayService;
        private readonly IMediaLibraryService _mediaLibraryService;

        private CancellationTokenSource _cancellationTokenSource;

        #region Properties

        public string AccountName
        {
            get
            {
                return _mainModel.CurrentAccount.Name.ToUpperInvariant();
            }
        }

        public string CurrentPath
        {
            get
            {
                return _mainModel.CurrentPath;
            }
        }

        public string Filename
        {
            get
            {
                return _mainModel.SelectedFile.Title;
            }
        }

        public string FileSize
        {
            get
            {
                if (_mainModel.SelectedFile.FileSize.HasValue)
                {
                    return _mainModel.SelectedFile.FileSize.Value.FormatAsSize();
                }

                return "(unknown)";
            }
        }

        public string FileModifiedDate
        {
            get
            {
                if (_mainModel.SelectedFile.ModifiedDate.HasValue)
                {
                    return _mainModel.SelectedFile.ModifiedDate.Value.ToString(CultureInfo.CurrentUICulture);
                }

                return "(unknown)";
            }
        }

        public Uri ThumbnailUri
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(_mainModel.SelectedFile.ThumbnailLink))
                    {
                        return new Uri(_mainModel.SelectedFile.ThumbnailLink, UriKind.Absolute);
                    }
                }
                catch
                {
                }

                return null;
            }
        }

        public bool HasThumbnail
        {
            get
            {
                return ThumbnailUri != null;
            }
        }

        public bool CanDownload
        {
            get
            {
                switch (_mainModel.SelectedFile.MimeType)
                {
                    case "image/jpeg":
                    case "image/png":
                        return true;

                    default:
                        return false;
                }
            }
        }

        public RelayCommand DownloadFileCommand { get; private set; }

        public RelayCommand<CancelEventArgs> BackKeyPressCommand { get; private set; }

        public bool IsBusy
        {
            get
            {
                return _systemTrayService.IsBusy;
            }
        }

        #endregion

        public ViewFileViewModel(IMainModel mainModel, IGoogleDriveService googleDriveService, IMessageBoxService messageBoxService, ISystemTrayService systemTrayService, IMediaLibraryService mediaLibraryService)
        {
            _mainModel = mainModel;
            _googleDriveService = googleDriveService;
            _messageBoxService = messageBoxService;
            _systemTrayService = systemTrayService;
            _mediaLibraryService = mediaLibraryService;

            DownloadFileCommand = new RelayCommand(DownloadFile, () => !IsBusy && CanDownload);

            BackKeyPressCommand = new RelayCommand<CancelEventArgs>(e =>
            {
                AbortCurrentCall(false);
            });
        }

        private async void DownloadFile()
        {
            try
            {
                AbortCurrentCall();

                await _mainModel.CheckToken(CancellationToken.None);

                _systemTrayService.SetProgressIndicator("Downloading file...");

                var fileData = await _googleDriveService.FileDownload(_mainModel.CurrentAccount.AuthToken, _mainModel.SelectedFile.DownloadUrl, CancellationToken.None);

                var destinationFilename = string.Format("{0}_{1:ddMMyyyyHHmmss}{2}", 
                    Path.GetFileNameWithoutExtension(_mainModel.SelectedFile.Title), 
                    DateTime.Now, 
                    Path.GetExtension(_mainModel.SelectedFile.Title));

                _mediaLibraryService.SavePicture(destinationFilename, fileData);

                _systemTrayService.HideProgressIndicator();

                _messageBoxService.Show("Image file downloaded and saved to phone successfully!", "File Downloaded");
            }
            catch (TaskCanceledException)
            {
                _systemTrayService.HideProgressIndicator();
            }
            catch (Exception)
            {
                _systemTrayService.HideProgressIndicator();

                _messageBoxService.Show("Unable to download file!", "Error");
            }
        }

        private void AbortCurrentCall(bool createNew = true)
        {
            _systemTrayService.HideProgressIndicator();

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            if (createNew)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
    }
}