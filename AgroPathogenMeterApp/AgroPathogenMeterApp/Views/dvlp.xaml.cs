using Microsoft.AppCenter.Analytics;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dvlp : ContentPage   //The developer options menu page
    {
        public Dvlp()
        {
            Analytics.TrackEvent("Developer opened");
            InitializeComponent();
        }

        private async void OnManualTestClicked(object sender, EventArgs e)   //Opens the manual tester to choose the test and parameters
        {
            await Navigation.PushAsync(new manTest { });
        }

        private async void OnRawDataClicked(object sender, EventArgs e)   //Opens a file selecter to view raw data from a chosen test
        {
            await Navigation.PushAsync(new RawData { });
        }

        private async void OnTestingClicked(object sender, EventArgs e)   //Opens up a small test screen to test a positive result
        {
            await Navigation.PushAsync(new TestingDb { });
        }

        private async void OnClearDbClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ClearDb { });
        }
    }
}