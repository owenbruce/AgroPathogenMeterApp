using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
using XamarinUniversity.Infrastructure;

namespace AgroPathogenMeterApp.Views
{
    public partial class Bluetooth : ContentPage
    {
        public Bluetooth()   //Allow the user to connect to the APM via bluetooth without having to run a test
        {
            Analytics.TrackEvent("Bluetooth Opened");
            InitializeComponent();
        }

        private async void OnBtClicked(object sender, EventArgs e)
        {
            try
            {
                bool simple = true;
                var platform = DependencyService.Get<IBtControl>();
                string btDatabase = platform.FilePath();   //Connect to bluetooth differently depending on the operating system being used
                await DisplayAlert("Error", btDatabase, "OK");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await DisplayAlert("Error", "Cannot find devices with error: " + ex, "OK");
            }

            ObservableDictionary<string, string> displayString = new ObservableDictionary<string, string>();
        }
    }
}