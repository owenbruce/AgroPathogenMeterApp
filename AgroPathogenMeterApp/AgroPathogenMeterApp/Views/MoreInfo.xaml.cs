using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter.Analytics;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MoreInfo : ContentPage
    {
        public MoreInfo()
        {
            Analytics.TrackEvent("More info opened");
            InitializeComponent();
        }
    }
}