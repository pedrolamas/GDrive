#if !WP8
using System;
#endif
using System.Collections.Generic;
using System.Linq;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using PedroLamas.GDrive.Model;

namespace PedroLamas.GDrive.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IMainModel _mainModel;
        private readonly INavigationService _navigationService;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IApplicationSettingsService _applicationSettingsService;
        private readonly IShellTileService _shellTileService;

        #region Properties

        public IEnumerable<AccountViewModel> AvailableAccounts
        {
            get
            {
                return _mainModel.AvailableAccounts.Select(x => new AccountViewModel(x));
            }
        }

        public bool EmptyAvailableAccounts
        {
            get
            {
                return _mainModel.AvailableAccounts.Count == 0;
            }
        }

        public RelayCommand NewAccountCommand { get; private set; }

        public RelayCommand<AccountViewModel> RemoveAccountCommand { get; private set; }

        public RelayCommand<AccountViewModel> OpenAccountCommand { get; private set; }

        public RelayCommand ShowAboutCommand { get; private set; }

        public RelayCommand PageLoadedCommand { get; private set; }

        #endregion

        public MainViewModel(IMainModel mainModel, INavigationService navigationService, IMessageBoxService messageBoxService, IApplicationSettingsService applicationSettingsService, IShellTileService shellTileService)
        {
            _mainModel = mainModel;
            _navigationService = navigationService;
            _messageBoxService = messageBoxService;
            _applicationSettingsService = applicationSettingsService;
            _shellTileService = shellTileService;

            NewAccountCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo("/View/AuthorizationPage.xaml");
            });

            RemoveAccountCommand = new RelayCommand<AccountViewModel>(account =>
            {
                _mainModel.AvailableAccounts.Remove(account.Model);
                _mainModel.Save();

                RefreshAccountsList();
            });

            OpenAccountCommand = new RelayCommand<AccountViewModel>(account =>
            {
                _mainModel.CurrentAccount = account.Model;
                _mainModel.ExecuteInitialLoad = true;

                _navigationService.NavigateTo("/View/ExplorerPage.xaml");
            });

            ShowAboutCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo("/View/AboutPage.xaml");
            });

            PageLoadedCommand = new RelayCommand(() =>
            {
                _mainModel.CurrentAccount = null;

                if (!_applicationSettingsService.Get<bool>("AcceptedDisclaimer", false))
                {
                    _applicationSettingsService.Set("AcceptedDisclaimer", true);
                    _applicationSettingsService.Save();

                    _messageBoxService.Show("You are advised to read the GDrive disclaimer before you continue.\n\nWould you like to read it now?\n\nYou can always find it later in the About page.", "Welcome to GDrive", new[] { "now", "later" }, buttonIndex =>
                    {
                        if (buttonIndex == 0)
                        {
                            _navigationService.NavigateTo("/View/AboutPage.xaml?disclaimer=true");
                        }
                    });
                }
            });

            MessengerInstance.Register<AvailableAccountsChangedMessage>(this, message =>
            {
                RefreshAccountsList();
            });

#if !WP8
            DispatcherHelper.RunAsync(UpdateTiles);
#endif
        }

        private void RefreshAccountsList()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                RaisePropertyChanged(() => AvailableAccounts);
                RaisePropertyChanged(() => EmptyAvailableAccounts);
            });
        }

#if !WP8
        private void UpdateTiles()
        {
            if (!_shellTileService.LiveTilesSupported)
            {
                return;
            }

            var primaryTile = _shellTileService.ActiveTiles.FirstOrDefault();

            if (primaryTile != null)
            {
                primaryTile.Update(new ShellTileServiceFlipTileData()
                {
                    SmallBackgroundImage = new Uri("/GDrive_159x159.png", UriKind.Relative),
                    BackgroundImage = new Uri("/GDrive_336x336.png", UriKind.Relative),
                    WideBackgroundImage = new Uri("/GDrive_691x336.png", UriKind.Relative)
                });
            }
        }
#endif
    }
}