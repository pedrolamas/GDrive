using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Extensions;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using PedroLamas.GDrive.Model;
using PedroLamas.GDrive.Service;

namespace PedroLamas.GDrive.ViewModel
{
    public class ExplorerViewModel : ViewModelBase
    {
        private const string GoogleDriveFilesListFields = "etag,items(description,downloadUrl,fileSize,id,thumbnailLink,labels,mimeType,modifiedDate,title),nextPageToken";
        private const string GoogleDriveFileFields = "description,fileSize,id,labels,mimeType,modifiedDate,title";

        private readonly IMainModel _mainModel;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly INavigationService _navigationService;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ISystemTrayService _systemTrayService;
        private readonly IPhotoChooserService _photoChooserService;

        private int _pivotSelectedIndex;
        private bool _isSelectionEnabled;
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

        public ObservableCollection<GoogleFileViewModel> Files { get; private set; }

        public ObservableCollection<GoogleFileViewModel> PictureFiles { get; private set; }

        public int PivotSelectedIndex
        {
            get
            {
                return _pivotSelectedIndex;
            }
            set
            {
                if (_pivotSelectedIndex == value)
                    return;

                _pivotSelectedIndex = value;

                RaisePropertyChanged(() => PivotSelectedIndex);
            }
        }

        public bool IsSelectionEnabled
        {
            get
            {
                return _isSelectionEnabled;
            }
            set
            {
                if (_isSelectionEnabled == value)
                    return;

                _isSelectionEnabled = value;

                RaisePropertyChanged(() => IsSelectionEnabled);
                RaisePropertyChanged(() => SelectedApplicationBarIndex);
            }
        }

        public int SelectedApplicationBarIndex
        {
            get
            {
                return _isSelectionEnabled ? 1 : 0;
            }
        }

        public RelayCommand<GoogleFileViewModel> OpenFileCommand { get; private set; }

        public RelayCommand<GoogleFileViewModel> ChangeStaredStatusCommand { get; private set; }

        public RelayCommand AddFileCommand { get; private set; }

        public RelayCommand EnableSelectionCommand { get; private set; }

        public RelayCommand RefreshFilesCommand { get; private set; }

        public RelayCommand<IList> DeleteFilesCommand { get; private set; }

        public RelayCommand CreateNewFolderCommand { get; private set; }

        public RelayCommand<GoogleFileViewModel> RenameFileCommand { get; private set; }

        public RelayCommand<GoogleFileViewModel> DeleteFileCommand { get; private set; }

        public RelayCommand ShowAboutCommand { get; private set; }

        public RelayCommand PageLoadedCommand { get; private set; }

        public RelayCommand<CancelEventArgs> BackKeyPressCommand { get; private set; }

        public bool IsBusy
        {
            get
            {
                return _systemTrayService.IsBusy;
            }
        }

        #endregion

        public ExplorerViewModel(IMainModel mainModel, IGoogleDriveService googleDriveService, INavigationService navigationService, IMessageBoxService messageBoxService, ISystemTrayService systemTrayService, IPhotoChooserService photoChooserService)
        {
            _mainModel = mainModel;
            _googleDriveService = googleDriveService;
            _navigationService = navigationService;
            _messageBoxService = messageBoxService;
            _systemTrayService = systemTrayService;
            _photoChooserService = photoChooserService;

            Files = new ObservableCollection<GoogleFileViewModel>();
            PictureFiles = new ObservableCollection<GoogleFileViewModel>();

            OpenFileCommand = new RelayCommand<GoogleFileViewModel>(file =>
            {
                if (IsSelectionEnabled)
                {
                    return;
                }

                OpenFile(file);
            });

            ChangeStaredStatusCommand = new RelayCommand<GoogleFileViewModel>(file =>
            {
                if (IsSelectionEnabled)
                {
                    return;
                }

                ChangeStaredStatus(file);
            });

            AddFileCommand = new RelayCommand(UploadFile);

            EnableSelectionCommand = new RelayCommand(() =>
            {
                if (IsBusy)
                {
                    return;
                }

                IsSelectionEnabled = true;
            });

            RefreshFilesCommand = new RelayCommand(RefreshFiles);

            DeleteFilesCommand = new RelayCommand<IList>(files =>
            {
                _messageBoxService.Show("You are about to delete the selected files. Do you wish to proceed?", "Delete files?", new[] { "delete", "cancel" }, button =>
                {
                    if (button != 0)
                        return;

                    var filesArray = files
                        .Cast<GoogleFileViewModel>()
                        .ToArray();

                    IsSelectionEnabled = false;

                    DeleteFiles(filesArray);
                });
            });

            CreateNewFolderCommand = new RelayCommand(CreateNewFolder);

            RenameFileCommand = new RelayCommand<GoogleFileViewModel>(RenameFile);

            DeleteFileCommand = new RelayCommand<GoogleFileViewModel>(file =>
            {
                _messageBoxService.Show(string.Format("You are about to delete '{0}'. Do you wish to proceed?", file.Title), "Delete file?", new[] { "delete", "cancel" }, button =>
                {
                    if (button != 0)
                        return;

                    DeleteFile(file);
                });
            });

            ShowAboutCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo("/View/AboutPage.xaml");
            });

            PageLoadedCommand = new RelayCommand(ExecuteInitialLoad);

            BackKeyPressCommand = new RelayCommand<CancelEventArgs>(e =>
            {
                GoogleDriveFile item;

                if (PivotSelectedIndex == 1)
                {
                    PivotSelectedIndex = 0;

                    e.Cancel = true;
                }
                else if (IsSelectionEnabled)
                {
                    IsSelectionEnabled = false;

                    e.Cancel = true;
                }
                else if (_mainModel.TryPop(out item))
                {
                    AbortCurrentCall();

                    RaisePropertyChanged(() => CurrentPath);

                    RefreshFiles();

                    e.Cancel = true;
                }
                else
                {
                    AbortCurrentCall(true);

                    Files.Clear();
                    PictureFiles.Clear();
                }
            });

            MessengerInstance.Register<RefreshFilesMessage>(this, message =>
            {
                DispatcherHelper.RunAsync(RefreshFiles);
            });
        }

        private async void ExecuteInitialLoad()
        {
            if (!_mainModel.ExecuteInitialLoad)
            {
                return;
            }

            _mainModel.ExecuteInitialLoad = false;

            AbortCurrentCall();

            try
            {
                await _mainModel.CheckToken(_cancellationTokenSource.Token);

                _systemTrayService.SetProgressIndicator("Reading drive info...");

                var about = await _googleDriveService.About(_mainModel.CurrentAccount.AuthToken, new GoogleDriveAboutRequest()
                {
                    ETag = _mainModel.CurrentAccount.Info != null ? _mainModel.CurrentAccount.Info.ETag : null
                }, _cancellationTokenSource.Token);

                if (about != null)
                {
                    _mainModel.CurrentAccount.Info = about;
                    _mainModel.Save();
                }

                await RefreshFilesAsync();
            }
            catch (TaskCanceledException)
            {
                _systemTrayService.HideProgressIndicator();
            }
            catch (Exception)
            {
                _systemTrayService.HideProgressIndicator();

                _messageBoxService.Show("Unable to get the drive information!", "Error");
            }
        }

        private void OpenFile(GoogleFileViewModel fileViewModel)
        {
            if (fileViewModel.IsFolder)
            {
                _mainModel.Push(fileViewModel.FileModel);

                RaisePropertyChanged(() => CurrentPath);

                RefreshFiles();
            }
            else
            {
                _mainModel.SelectedFile = fileViewModel.FileModel;

                _navigationService.NavigateTo("/View/ViewFilePage.xaml");
            }
        }

        private async void ChangeStaredStatus(GoogleFileViewModel fileViewModel)
        {
            if (IsBusy)
            {
                return;
            }

            AbortCurrentCall();

            try
            {
                await _mainModel.CheckToken(_cancellationTokenSource.Token);

                _systemTrayService.SetProgressIndicator("Changing file star state...");

                var currentStaredtatus = fileViewModel.FileModel.Labels.Starred;

                var fileModel = await _googleDriveService.FilesUpdate(_mainModel.CurrentAccount.AuthToken, fileViewModel.Id, new GoogleDriveFilesUpdateRequest()
                {
                    File = new GoogleDriveFile()
                    {
                        Labels = new GoogleDriveLabels()
                        {
                            Starred = !currentStaredtatus.GetValueOrDefault()
                        }
                    },
                    Fields = GoogleDriveFileFields
                }, _cancellationTokenSource.Token);

                fileViewModel.FileModel = fileModel;

                _systemTrayService.HideProgressIndicator();
            }
            catch (TaskCanceledException)
            {
                _systemTrayService.HideProgressIndicator();
            }
            catch (Exception)
            {
                _systemTrayService.HideProgressIndicator();

                _messageBoxService.Show("Unable to change the file star state!", "Error");
            }
        }

        private void UploadFile()
        {
            AbortCurrentCall();

            _photoChooserService.Show(true, result =>
            {
                if (result.TaskResult != Microsoft.Phone.Tasks.TaskResult.OK)
                    return;

                DispatcherHelper.RunAsync(async () =>
                {
                    try
                    {
                        await _mainModel.CheckToken(_cancellationTokenSource.Token);

                        _systemTrayService.SetProgressIndicator("Uploading the file...");

                        var fileModel = await _googleDriveService.FilesInsert(_mainModel.CurrentAccount.AuthToken, new GoogleDriveFilesInsertRequest()
                        {
                            Filename = System.IO.Path.GetFileName(result.OriginalFileName),
                            FileContent = result.ChosenPhoto.ToArray(),
                            FolderId = _mainModel.CurrentFolderId,
                            Fields = GoogleDriveFileFields
                        }, _cancellationTokenSource.Token);

                        var googleFileViewModel = new GoogleFileViewModel(fileModel);

                        Files.Add(googleFileViewModel);

                        if (!string.IsNullOrEmpty(googleFileViewModel.ThumbnailLink))
                        {
                            PictureFiles.Add(googleFileViewModel);
                        }

                        _systemTrayService.HideProgressIndicator();
                    }
                    catch (TaskCanceledException)
                    {
                        _systemTrayService.HideProgressIndicator();
                    }
                    catch (Exception)
                    {
                        _systemTrayService.HideProgressIndicator();

                        _messageBoxService.Show("Unable to upload the file!", "Error");
                    }
                });
            });
        }

        private async void RefreshFiles()
        {
            await RefreshFilesAsync();
        }

        private void CreateNewFolder()
        {
            AbortCurrentCall();

            _navigationService.NavigateTo("/View/NewFolderPage.xaml");
        }

        private void RenameFile(GoogleFileViewModel fileViewModel)
        {
            _mainModel.SelectedFile = fileViewModel.FileModel;

            _navigationService.NavigateTo("/View/RenameFilePage.xaml");
        }

        private async void DeleteFile(GoogleFileViewModel fileViewModel)
        {
            AbortCurrentCall();

            await DeleteFileAsync(fileViewModel);

            _systemTrayService.HideProgressIndicator();
        }

        private async void DeleteFiles(IEnumerable<GoogleFileViewModel> filesToDelete)
        {
            AbortCurrentCall();

            foreach (var fileViewModel in filesToDelete)
            {
                if (!await DeleteFileAsync(fileViewModel))
                {
                    break;
                }
            }

            _systemTrayService.HideProgressIndicator();
        }

        private async Task RefreshFilesAsync()
        {
            AbortCurrentCall();

            try
            {
                var currentFolderId = _mainModel.CurrentFolderId;

                await _mainModel.CheckToken(_cancellationTokenSource.Token);

                _systemTrayService.SetProgressIndicator("Refreshing the file list...");

                Files.Clear();
                PictureFiles.Clear();

                string pageToken = null;

                while (true)
                {
                    var filesListResponse = await _googleDriveService.FilesList(_mainModel.CurrentAccount.AuthToken, new GoogleDriveFilesListRequest()
                    {
                        Query = "trashed=false and '{0}' in parents".FormatWith(currentFolderId),
                        Fields = GoogleDriveFilesListFields,
                        PageToken = pageToken
                    }, _cancellationTokenSource.Token);

                    if (filesListResponse.Items != null)
                    {
                        foreach (var item in filesListResponse.Items)
                        {
                            var googleFileViewModel = new GoogleFileViewModel(item);

                            Files.Add(googleFileViewModel);

                            if (item != null && !string.IsNullOrEmpty(item.ThumbnailLink))
                            {
                                PictureFiles.Add(googleFileViewModel);
                            }
                        }
                    }

                    pageToken = filesListResponse.NextPageToken;

                    if (string.IsNullOrEmpty(pageToken))
                    {
                        _systemTrayService.HideProgressIndicator();

                        return;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                _systemTrayService.HideProgressIndicator();
            }
            catch (Exception)
            {
                _systemTrayService.HideProgressIndicator();

                _messageBoxService.Show("Unable to update the file list!", "Error");
            }
        }

        private async Task<bool> DeleteFileAsync(GoogleFileViewModel fileViewModel)
        {
            try
            {
                await _mainModel.CheckToken(_cancellationTokenSource.Token);

                _systemTrayService.SetProgressIndicator(string.Format("Deleting: {0}...", fileViewModel.Title));

                await _googleDriveService.FilesDelete(_mainModel.CurrentAccount.AuthToken, fileViewModel.Id, _cancellationTokenSource.Token);

                lock (Files)
                {
                    if (Files.Contains(fileViewModel))
                    {
                        Files.Remove(fileViewModel);
                    }
                }

                return true;
            }
            catch
            {
                _systemTrayService.HideProgressIndicator();

                _messageBoxService.Show(string.Format("Unable to delete '{0}'!", fileViewModel.Title), "Error");
            }

            return false;
        }

        private void AbortCurrentCall(bool hideSystemTray = false)
        {
            if (hideSystemTray)
            {
                _systemTrayService.HideProgressIndicator();
            }

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}