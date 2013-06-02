using System;
using System.Linq;
using System.Threading;
using System.Windows.Navigation;
using Cimbalino.Phone.Toolkit.Extensions;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using PedroLamas.GDrive.Model;
using PedroLamas.GDrive.Service;

namespace PedroLamas.GDrive.ViewModel
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

                if (e.Uri == null || e.Uri.Host != "localhost")
                    return;

                ShowBrowser = false;

                WebBrowserSourceUri = new Uri("https://accounts.google.com/Logout");

                var queryString = e.Uri.QueryString();

                ExchangeAuthorizationCode(queryString.GetValue("code"));
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

        private async void ExchangeAuthorizationCode(string authorizationCode)
        {
            try
            {
                var authToken = await _googleAuthService.ExchangeAuthorizationCode(authorizationCode, CancellationToken.None);

                var userInfo = await _googleOAuth2Service.GetUserInfo(authToken, CancellationToken.None);

                _systemTrayService.HideProgressIndicator();

                var email = userInfo.EMail;

                var oldAccount = _mainModel.AvailableAccounts.FirstOrDefault(x => x.Name == email);

                if (oldAccount != null)
                {
                    _mainModel.AvailableAccounts.Remove(oldAccount);
                }

                _mainModel.AvailableAccounts.Add(new AccountModel(email, authToken));
                _mainModel.Save();

                MessengerInstance.Send(new AvailableAccountsChangedMessage());

                _navigationService.GoBack();
            }
            catch
            {
                _systemTrayService.HideProgressIndicator();

                _messageBoxService.Show("Unable to retrieve the access tokens!", "Error");
            }
        }
    }
}