using Cimbalino.Phone.Toolkit.Extensions;
using Microsoft.Phone.Controls;

namespace PedroLamas.GDrive.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            this.ResetLanguageWithCurrentCulture();

            InitializeComponent();
        }
    }
}