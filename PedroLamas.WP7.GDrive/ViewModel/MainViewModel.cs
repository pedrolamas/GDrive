using System.Collections.Generic;
using System.Linq;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using PedroLamas.WP7.GDrive.Model;

namespace PedroLamas.WP7.GDrive.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IMainModel _mainModel;
        private readonly INavigationService _navigationService;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IApplicationSettingsService _applicationSettingsService;

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

        public RelayCommand<AccountViewModel> OpenAccountCommand { get; private set; }

        public RelayCommand ShowAboutCommand { get; private set; }

        public RelayCommand PageLoadedCommand { get; private set; }

        #endregion

        public MainViewModel(IMainModel mainModel, INavigationService navigationService, IMessageBoxService messageBoxService, IApplicationSettingsService applicationSettingsService)
        {
            _mainModel = mainModel;
            _navigationService = navigationService;
            _messageBoxService = messageBoxService;
            _applicationSettingsService = applicationSettingsService;

            NewAccountCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo("/View/AuthorizationPage.xaml");
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

                    _messageBoxService.Show("You are advised to read the GDrive disclaimer before you continue.\n\nWould you like to read it now?\n\nYou can always find it later in the About page.", "Welcome to GDrive", new string[] { "now", "later" }, buttonIndex =>
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
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    RaisePropertyChanged(() => AvailableAccounts);
                    RaisePropertyChanged(() => EmptyAvailableAccounts);
                });
            });
        }
    }
}