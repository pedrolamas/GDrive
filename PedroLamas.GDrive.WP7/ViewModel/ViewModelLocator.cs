using System;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using PedroLamas.GDrive.Model;
using PedroLamas.GDrive.Service;

namespace PedroLamas.GDrive.ViewModel
{
    public class ViewModelLocator
    {
        // TODO In order to build and use this app, you have to set the following two constants to your Google API Client Id and Client Secret. You can find more information on this subject here: https://github.com/PedroLamas/GDrive

        private const string GoogleDriveApi_ClientId = "";
        private const string GoogleDriveApi_ClientSecret = "";

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            Register<INavigationService, NavigationService>();
            Register<IStorageService, StorageService>();
            Register<IMessageBoxService, MessageBoxService>();
            Register<ISystemTrayService, SystemTrayService>();
            Register<IPhotoChooserService, PhotoChooserWithCameraService>();
            Register<IApplicationSettingsService, ApplicationSettingsService>();
            Register<IWebBrowserService, WebBrowserService>();
            Register<IMarketplaceReviewService, MarketplaceReviewService>();
            Register<IMarketplaceSearchService, MarketplaceSearchService>();
            Register<IShareLinkService, ShareLinkService>();
            Register<IShellTileService, ShellTileService>();
            Register<IMediaLibraryService, MediaLibraryService>();

            Register<IMainModel, MainModel>();
            Register<IGoogleClientSettings>(() => new GoogleClientSettings()
            {
                ClientId = GoogleDriveApi_ClientId,
                ClientSecret = GoogleDriveApi_ClientSecret,
                Scopes = new[]
                {
                    "https://www.googleapis.com/auth/userinfo.email",
                    "https://www.googleapis.com/auth/drive"
                }
            });
            Register<IGoogleAuthService, GoogleAuthService>();
            Register<IGoogleOAuth2Service, GoogleOAuth2Service>();
            Register<IGoogleDriveService, GoogleDriveService>();

            Register<MainViewModel>();
            Register<ExplorerViewModel>();
            Register<NewFolderViewModel>();
            Register<RenameFileViewModel>();
            Register<ViewFileViewModel>();
            Register<AuthorizationViewModel>();
            Register<AboutViewModel>();
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

        public RenameFileViewModel RenameFile
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RenameFileViewModel>();
            }
        }

        public ViewFileViewModel ViewFile
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewFileViewModel>();
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

        #region Auxiliary Methods

        private void Register<TInterface, TClass>()
            where TInterface : class
            where TClass : class
        {
            if (!SimpleIoc.Default.IsRegistered<TInterface>())
            {
                SimpleIoc.Default.Register<TInterface, TClass>();
            }
        }

        private void Register<TClass>() where TClass : class
        {
            if (!SimpleIoc.Default.IsRegistered<TClass>())
            {
                SimpleIoc.Default.Register<TClass>();
            }
        }

        private void Register<TClass>(Func<TClass> factory) where TClass : class
        {
            if (!SimpleIoc.Default.IsRegistered<TClass>())
            {
                SimpleIoc.Default.Register(factory);
            }
        }

        #endregion
    }
}