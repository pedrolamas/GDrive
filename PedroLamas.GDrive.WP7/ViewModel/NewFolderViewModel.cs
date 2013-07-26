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
    public class NewFolderViewModel : ViewModelBase
    {
        private const string GoogleDriveFileFields = "description,etag,fileSize,id,labels,mimeType,modifiedDate,title";

        private readonly IMainModel _mainModel;
        private readonly INavigationService _navigationService;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly ISystemTrayService _systemTrayService;
        private readonly IMessageBoxService _messageBoxService;

        private string _folderName;

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

        public RelayCommand CreateNewFolderCommand { get; set; }

        public string FolderName
        {
            get
            {
                return _folderName;
            }
            set
            {
                if (_folderName == value)
                    return;

                _folderName = value;

                RaisePropertyChanged(() => FolderName);
            }
        }

        public bool IsBusy
        {
            get
            {
                return _systemTrayService.IsBusy;
            }
        }

        #endregion

        public NewFolderViewModel(IMainModel mainModel, INavigationService navigationService, IGoogleDriveService googleDriveService, ISystemTrayService systemTrayService, IMessageBoxService messageBoxService)
        {
            _mainModel = mainModel;
            _navigationService = navigationService;
            _googleDriveService = googleDriveService;
            _systemTrayService = systemTrayService;
            _messageBoxService = messageBoxService;

            CreateNewFolderCommand = new RelayCommand(CreateNewFolder, () => !IsBusy);
        }

        private async void CreateNewFolder()
        {
            try
            {
                await _mainModel.CheckToken(CancellationToken.None);
            
                _systemTrayService.SetProgressIndicator("Creating new folder...");

                var file = await _googleDriveService.FilesInsert(_mainModel.CurrentAccount.AuthToken, new GoogleDriveFilesInsertRequest()
                {
                    Filename = FolderName,
                    FolderId = _mainModel.CurrentFolderId,
                    Fields = GoogleDriveFileFields
                }, CancellationToken.None);

                FolderName = string.Empty;

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

                _messageBoxService.Show("Unable to create the new folder!", "Error");
            }
        }
    }
}