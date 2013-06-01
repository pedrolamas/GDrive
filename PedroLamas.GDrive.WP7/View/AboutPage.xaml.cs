using Cimbalino.Phone.Toolkit.Extensions;
using Microsoft.Phone.Controls;

namespace PedroLamas.GDrive.View
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            this.ResetLanguageWithCurrentCulture();

            InitializeComponent();
        }
    }
}