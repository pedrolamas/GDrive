using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using Newtonsoft.Json;
using PedroLamas.GDrive.Service;

namespace PedroLamas.GDrive.Model
{
    public class MainModel : IMainModel
    {
        private const string FILENAME = @"data.txt";

        private readonly object _lock = new object();

        private readonly IGoogleAuthService _googleAuthService;
        private readonly IStorageService _storageService;
        private readonly ISystemTrayService _systemTrayService;

        private readonly GoogleDriveBreadcrumbs _breadcrumbs = new GoogleDriveBreadcrumbs();

        #region Properties

        public IList<AccountModel> AvailableAccounts { get; private set; }

        public AccountModel CurrentAccount { get; set; }

        public bool ExecuteInitialLoad { get; set; }

        public GoogleDriveFile CurrentFolder
        {
            get
            {
                GoogleDriveFile folder;

                return _breadcrumbs.TryPeek(out folder) ? folder : null;
            }
        }

        public GoogleDriveFile SelectedFile { get; set; }

        public string CurrentFolderId
        {
            get
            {
                GoogleDriveFile folder;

                return _breadcrumbs.TryPeek(out folder) ? folder.Id : CurrentAccount.Info.RootFolderId;
            }
        }

        public string CurrentPath
        {
            get
            {
                return _breadcrumbs.CurrentPath;
            }
        }

        #endregion

        public MainModel(IGoogleAuthService googleAuthService, IStorageService storageService, ISystemTrayService systemTrayService)
        {
            _googleAuthService = googleAuthService;
            _storageService = storageService;
            _systemTrayService = systemTrayService;

            Load();
        }

        public async Task CheckToken(CancellationToken cancellationToken)
        {
            var authToken = CurrentAccount.AuthToken;

            if (DateTime.Now > authToken.ExpiresDateTime)
            {
                _systemTrayService.SetProgressIndicator("Refreshing the access token...");

                CurrentAccount.AuthToken = await _googleAuthService.RefreshToken(authToken, cancellationToken);

                Save();
            }
        }

        public void Clear()
        {
            _breadcrumbs.Clear();
        }

        public void Push(GoogleDriveFile item)
        {
            _breadcrumbs.Push(item);
        }

        public bool TryPeek(out GoogleDriveFile item)
        {
            return _breadcrumbs.TryPeek(out item);
        }

        public bool TryPop(out GoogleDriveFile item)
        {
            return _breadcrumbs.TryPop(out item);
        }

        public void Save()
        {
            lock (_lock)
            {
                _storageService.WriteAllText(FILENAME, JsonConvert.SerializeObject(AvailableAccounts));
            }
        }

        private void Load()
        {
            if (_storageService.FileExists(FILENAME))
            {
                AvailableAccounts = JsonConvert.DeserializeObject<List<AccountModel>>(_storageService.ReadAllText(FILENAME));
            }
            else
            {
                AvailableAccounts = new List<AccountModel>();
            }
        }
    }
}