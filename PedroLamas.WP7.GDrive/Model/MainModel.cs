using System;
using System.Collections.Generic;
using Cimbalino.Phone.Toolkit.Services;
using Newtonsoft.Json;
using PedroLamas.WP7.GDrive.Service;
using PedroLamas.WP7.ServiceModel;

namespace PedroLamas.WP7.GDrive.Model
{
    public class MainModel : IMainModel
    {
        private const string FILENAME = @"data.txt";

        private readonly IGoogleAuthService _googleAuthService;
        private readonly IStorageService _storageService;
        private readonly ISystemTrayService _systemTrayService;

        private readonly object _lock = new object();

        #region Properties

        public IList<AccountModel> AvailableAccounts { get; private set; }

        public AccountModel CurrentAccount { get; set; }

        public bool ExecuteInitialLoad { get; set; }

        #endregion

        public MainModel(IGoogleAuthService googleAuthService, IStorageService storageService, ISystemTrayService systemTrayService)
        {
            _googleAuthService = googleAuthService;
            _storageService = storageService;
            _systemTrayService = systemTrayService;

            Load();
        }

        public void CheckTokenAndExecute<T>(Action<GoogleAuthToken, ResultCallback<T>, object> action, ResultCallback<T> callback, object state)
        {
            lock (_lock)
            {
                var authToken = CurrentAccount.AuthToken;

                if (DateTime.Now > authToken.ExpiresDateTime)
                {
                    _systemTrayService.SetProgressIndicator("Refreshing the access token...");

                    _googleAuthService.RefreshToken(authToken, result =>
                    {
                        try
                        {
                            if (result.Status != ResultStatus.Completed)
                            {
                                callback(new Result<T>(result.Status, result.State));
                            }

                            var newAuthToken = result.Data;

                            CurrentAccount.AuthToken = newAuthToken;

                            Save();

                            action(newAuthToken, callback, result.State);
                        }
                        catch (Exception ex)
                        {
                            callback(new Result<T>(ex, result.State));
                        }
                    }, state);
                }
                else
                {
                    action(authToken, callback, state);
                }
            }
        }

        private void Load()
        {
            if (_storageService.FileExists(FILENAME))
                AvailableAccounts = JsonConvert.DeserializeObject<List<AccountModel>>(_storageService.ReadAllText(FILENAME));
            else
                AvailableAccounts = new List<AccountModel>();
        }

        public void Save()
        {
            _storageService.WriteAllText(FILENAME, JsonConvert.SerializeObject(AvailableAccounts));
        }
    }
}