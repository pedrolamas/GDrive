using System;
using System.Linq;
using System.Windows.Navigation;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using PedroLamas.WP7.GDrive.Model;
using PedroLamas.WP7.GDrive.Service;
using PedroLamas.WP7.ServiceModel;

namespace PedroLamas.WP7.GDrive.ViewModel
{
    public class AuthorizationViewModel : ViewModelBase
    {
        private readonly IMainModel _mainModel;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IGoogleOAuth2Service _googleOAuth2Service;
        private readonly INavigationService _navigationService;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ISystemTrayService _systemTrayService;

        private bool _showBrowser;
        private Uri _webBrowserSourceUri;

        #region Properties

        public bool ShowBrowser
        {
            get
            {
                return _showBrowser;
            }
            set
            {
                if (_showBrowser == value)
                    return;

                _showBrowser = value;

                RaisePropertyChanged(() => ShowBrowser);
            }
        }

        public Uri WebBrowserSourceUri
        {
            get
            {
                return _webBrowserSourceUri;
            }
            set
            {
                if (_webBrowserSourceUri == value)
                    return;

                _webBrowserSourceUri = value;

                RaisePropertyChanged(() => WebBrowserSourceUri);
            }
        }

        public RelayCommand PageLoadedCommand { get; private set; }

        public RelayCommand<NavigatingEventArgs> WebBrowserNavigatingCommand { get; private set; }

        public RelayCommand<NavigationEventArgs> WebBrowserNavigatedCommand { get; private set; }

        public RelayCommand<NavigationFailedEventArgs> WebBrowserNavigationFailedCommand { get; private set; }

        #endregion

        public AuthorizationViewModel(IMainModel mainModel, IGoogleAuthService googleAuthService, IGoogleOAuth2Service googleOAuth2Service, INavigationService navigationService, IMessageBoxService messageBoxService, ISystemTrayService systemTrayService)
        {
            _mainModel = mainModel;
            _googleAuthService = googleAuthService;
            _googleOAuth2Service = googleOAuth2Service;
            _navigationService = navigationService;
            _messageBoxService = messageBoxService;
            _systemTrayService = systemTrayService;

            PageLoadedCommand = new RelayCommand(() =>
            {
                ShowBrowser = true;

                WebBrowserSourceUri = _googleAuthService.GetAuthUri();
            });

            WebBrowserNavigatingCommand = new RelayCommand<NavigatingEventArgs>(e =>
            {
                if (_showBrowser)
                {
                    _systemTrayService.SetProgressIndicator("");
                }

                if (e.Uri.Host != "localhost")
                    return;

                ShowBrowser = false;

                WebBrowserSourceUri = new Uri("https://accounts.google.com/Logout");

                var queryValues = e.Uri.Query.TrimStart('?').Split('&')
                    .Select(x => x.Split('='))
                    .ToDictionary(x => x[0], x => x.Length > 0 ? Uri.UnescapeDataString(x[1]) : null);

                ExchangeAuthorizationCode(queryValues["code"]);
            });

            WebBrowserNavigatedCommand = new RelayCommand<NavigationEventArgs>(e =>
            {
                if (_showBrowser)
                {
                    _systemTrayService.HideProgressIndicator();
                }
            });

            WebBrowserNavigationFailedCommand = new RelayCommand<NavigationFailedEventArgs>(e =>
            {
                _systemTrayService.HideProgressIndicator();
            });
        }

        private void ExchangeAuthorizationCode(string authorizationCode)
        {
            _googleAuthService.ExchangeAuthorizationCode(authorizationCode, result =>
            {
                switch (result.Status)
                {
                    case ResultStatus.Aborted:
                        break;

                    case ResultStatus.Completed:
                        var authToken = result.Data;

                        _googleOAuth2Service.GetUserInfo(authToken, result2 =>
                        {
                            if (result2.Status == ResultStatus.Aborted)
                            {
                                return;
                            }

                            _systemTrayService.HideProgressIndicator();

                            if (result2.Status != ResultStatus.Completed)
                            {
                                _messageBoxService.Show("Unable to retrieve the account email address!", "Error");

                                return;
                            }

                            var email = result2.Data.EMail;

                            var oldAccount = _mainModel.AvailableAccounts.FirstOrDefault(x => x.Name == email);

                            if (oldAccount != null)
                            {
                                _mainModel.AvailableAccounts.Remove(oldAccount);
                            }

                            _mainModel.AvailableAccounts.Add(new AccountModel(email, authToken));
                            _mainModel.Save();

                            MessengerInstance.Send(new AvailableAccountsChangedMessage());

                            _navigationService.GoBack();
                        }, null);

                        break;

                    default:
                        _systemTrayService.HideProgressIndicator();

                        _messageBoxService.Show("Unable to retrieve the access tokens!", "Error");

                        break;
                }
            }, null);
        }
    }
}