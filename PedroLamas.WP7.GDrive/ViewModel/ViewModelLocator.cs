using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using PedroLamas.WP7.GDrive.Model;
using PedroLamas.WP7.GDrive.Service;

namespace PedroLamas.WP7.GDrive.ViewModel
{
    public class ViewModelLocator
    {
        // TODO In order to build and use this app, you have to set the following two constants to your Google API Client Id and Client Secret. You can find more information on this subject here: https://github.com/PedroLamas/GDrive

        private const string GoogleDriveApi_ClientId = "";
        private const string GoogleDriveApi_ClientSecret = "";

        private readonly IGoogleClientSettings _clientSettings;

        public ViewModelLocator()
        {
            _clientSettings = new GoogleClientSettings()
            {
                ClientId = GoogleDriveApi_ClientId,
                ClientSecret = GoogleDriveApi_ClientSecret,
                Scopes = new string[]
                {
                    "https://www.googleapis.com/auth/userinfo.email",
                    "https://www.googleapis.com/auth/drive"
                }
            };

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (SimpleIoc.Default.IsRegistered<INavigationService>())
            {
                return;
            }

            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<IStorageService, StorageService>();
            SimpleIoc.Default.Register<IMessageBoxService, MessageBoxService>();
            SimpleIoc.Default.Register<ISystemTrayService, SystemTrayService>();
            SimpleIoc.Default.Register<IPhotoChooserService, PhotoChooserWithCameraService>();
            SimpleIoc.Default.Register<IApplicationSettingsService, ApplicationSettingsService>();
            SimpleIoc.Default.Register<IWebBrowserService, WebBrowserService>();
            SimpleIoc.Default.Register<IMarketplaceReviewService, MarketplaceReviewService>();
            SimpleIoc.Default.Register<IMarketplaceSearchService, MarketplaceSearchService>();
            SimpleIoc.Default.Register<IShareLinkService, ShareLinkService>();

            SimpleIoc.Default.Register<IMainModel, MainModel>();
            SimpleIoc.Default.Register<IGoogleClientSettings>(() => _clientSettings);
            SimpleIoc.Default.Register<IGoogleAuthService, GoogleAuthService>();
            SimpleIoc.Default.Register<IGoogleOAuth2Service, GoogleOAuth2Service>();
            SimpleIoc.Default.Register<IGoogleDriveService, GoogleDriveService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ExplorerViewModel>();
            SimpleIoc.Default.Register<NewFolderViewModel>();
            SimpleIoc.Default.Register<AuthorizationViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ExplorerViewModel Explorer
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ExplorerViewModel>();
            }
        }

        public NewFolderViewModel NewFolder
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NewFolderViewModel>();
            }
        }

        public AuthorizationViewModel Authorization
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AuthorizationViewModel>();
            }
        }

        public AboutViewModel About
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AboutViewModel>();
            }
        }

        public static void Cleanup()
        {
        }
    }
}