using System.Linq;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PedroLamas.WP7.GDrive.Model;
using PedroLamas.WP7.GDrive.Service;
using PedroLamas.WP7.ServiceModel;

namespace PedroLamas.WP7.GDrive.ViewModel
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
                return "/" + string.Join("/", _mainModel.CurrentAccount.PathBreadcrumbs
                    .Select(x => _mainModel.CurrentAccount.Files[x.Id].Title)
                    .Reverse()
                    .ToArray());
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

            CreateNewFolderCommand = new RelayCommand(() =>
            {
                CreateNewFolder();
            }, () => !IsBusy);
        }

        private void CreateNewFolder()
        {
            _systemTrayService.SetProgressIndicator("Creating new folder...");

            _mainModel.CheckTokenAndExecute<GoogleDriveFile>((authToken, callback, state) =>
            {
                _googleDriveService.FilesInsert(authToken, new GoogleDriveFilesInsertRequest()
                {
                    Filename = FolderName,
                    FolderId = _mainModel.CurrentAccount.CurrentFolder.Id,
                    Fields = GoogleDriveFileFields
                }, callback, state);
            }, result =>
            {
                _systemTrayService.HideProgressIndicator();

                switch (result.Status)
                {
                    case ResultStatus.Completed:
                    case ResultStatus.Empty:
                        FolderName = "";

                        MessengerInstance.Send(new RefreshFilesMessage());

                        _navigationService.GoBack();

                        break;

                    default:
                        _messageBoxService.Show("Unable to create the new folder!", "Error");

                        break;
                }
            }, null);
        }
    }
}