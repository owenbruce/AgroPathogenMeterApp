using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RawData : ContentPage   //Allows for developers to view the raw data
    {
        public RawData()   //Allow a developer to view the raw data
        {
            Analytics.TrackEvent("Raw Data Opened");
            InitializeComponent();
        }
    }
}