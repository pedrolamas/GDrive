using Cimbalino.Phone.Toolkit.Extensions;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PedroLamas.GDrive.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IWebBrowserService _webBrowserService;
        private readonly IMarketplaceReviewService _marketplaceReviewService;
        private readonly IMarketplaceSearchService _marketplaceSearchService;
        private readonly IShareLinkService _shareLinkService;

        private int _pivotSelectedIndex;

        #region Properties

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

        public RelayCommand OpenHomepageCommand { get; private set; }

        public RelayCommand OpenTwitterCommand { get; private set; }

        public RelayCommand RateApplicationCommand { get; private set; }

        public RelayCommand ShareApplicationCommand { get; private set; }

        public RelayCommand MarketplaceSearchCommand { get; private set; }

        public RelayCommand PageLoadedCommand { get; private set; }

        #endregion

        public AboutViewModel(INavigationService navigationService, IWebBrowserService webBrowserService, IMarketplaceReviewService marketplaceReviewService, IMarketplaceSearchService marketplaceSearchService, IShareLinkService shareLinkService)
        {
            _navigationService = navigationService;
            _webBrowserService = webBrowserService;
            _marketplaceReviewService = marketplaceReviewService;
            _marketplaceSearchService = marketplaceSearchService;
            _shareLinkService = shareLinkService;

            OpenHomepageCommand = new RelayCommand(() =>
            {
                _webBrowserService.Show("http://www.pedrolamas.com");
            });

            OpenTwitterCommand = new RelayCommand(() =>
            {
                _webBrowserService.Show("http://twitter.com/pedrolamas");
            });

            RateApplicationCommand = new RelayCommand(() =>
            {
                _marketplaceReviewService.Show();
            });

            ShareApplicationCommand = new RelayCommand(() =>
            {
                _shareLinkService.Show("GDrive", "GDrive: Google Drive on your Windows Phone! via @pedrolamas", "http://windowsphone.com/s?appid=c945c809-5e5d-4db3-b4c9-70c8cebd5235");
            });

            MarketplaceSearchCommand = new RelayCommand(() =>
            {
                _marketplaceSearchService.Show("Pedro Lamas");
            });

            PageLoadedCommand = new RelayCommand(() =>
            {
                PivotSelectedIndex = string.IsNullOrEmpty(_navigationService.QueryString.GetValue("disclaimer")) ? 0 : 1;
            });
        }
    }
}