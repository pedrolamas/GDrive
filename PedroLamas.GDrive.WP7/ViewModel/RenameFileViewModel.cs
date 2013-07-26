using System;
using System.Threading;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PedroLamas.GDrive.Model;
using PedroLamas.GDrive.Service;

namespace PedroLamas.GDrive.ViewModel
{
    public class RenameFileViewModel : ViewModelBase
    {
        private const string GoogleDriveFileFields = "description,etag,fileSize,id,labels,mimeType,modifiedDate,title";

        private readonly IMainModel _mainModel;
        private readonly INavigationService _navigationService;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly ISystemTrayService _systemTrayService;
        private readonly IMessageBoxService _messageBoxService;
        private string _fileName;

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

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                if (_fileName == value)
                    return;

                _fileName = value;

                RaisePropertyChanged(() => FileName);
            }
        }

        public RelayCommand RenameFileCommand { get; private set; }

        public RelayCommand PageLoadedCommand { get; private set; }

        public bool IsBusy
        {
            get
            {
                return _systemTrayService.IsBusy;
            }
        }

        #endregion

        public RenameFileViewModel(IMainModel mainModel, INavigationService navigationService, IGoogleDriveService googleDriveService, ISystemTrayService systemTrayService, IMessageBoxService messageBoxService)
        {
            _mainModel = mainModel;
            _navigationService = navigationService;
            _googleDriveService = googleDriveService;
            _systemTrayService = systemTrayService;
            _messageBoxService = messageBoxService;

            RenameFileCommand = new RelayCommand(RenameFile, () => !IsBusy);

            PageLoadedCommand = new RelayCommand(() =>
            {
                FileName = _mainModel.SelectedFile.Title;
            });
        }

        private async void RenameFile()
        {
            try
            {
                await _mainModel.CheckToken(CancellationToken.None);

                _systemTrayService.SetProgressIndicator("Renaming the file...");

                var file = await _googleDriveService.FilesUpdate(_mainModel.CurrentAccount.AuthToken, _mainModel.SelectedFile.Id, new GoogleDriveFilesUpdateRequest()
                {
                    File = new GoogleDriveFile()
                    {
                        Title = FileName
                    },
                    Fields = GoogleDriveFileFields
                }, CancellationToken.None);

                _mainModel.SelectedFile = null;

                MessengerInstance.Send(new RefreshFilesMessage());

                _navigationService.GoBack();
            }
            catch (TaskCanceledException)
            {
                _systemTrayService.HideProgressIndicator();
            }
            catch (Exception)
            {
                _systemTrayService.HideProgressIndicator();

                _messageBoxService.Show("Unable to rename the file!", "Error");
            }
        }
    }
}