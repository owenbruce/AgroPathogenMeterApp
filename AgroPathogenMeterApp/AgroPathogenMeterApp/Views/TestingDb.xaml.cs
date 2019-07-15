using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestingDb : ContentPage
    {
        public TestingDb()   //Test a positive scan
        {
            Analytics.TrackEvent("Testing Database opened");
            InitializeComponent();
        }

        private async void OnViewResultClicked(object sender, EventArgs e)   //Set the paramters for a positive result and open up the screen to view all the data
        {
            ScanDatabase scan = new ScanDatabase
            {
                AmountBacteria = 0,
                ConcentrationBacteria = 0,
                Date = DateTime.Now,
                IsInfected = true,
                VoltamType = "Positive Test"
            };
            await App.Database.SaveScanAsync(scan);
            try
            {
                bool simple = true;
                //DependencyService.Get<IBtControl>().Connect(simple);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed with error message: " + ex, "OK");
                Crashes.TrackError(ex);
            }

            await Navigation.PushAsync(new AllData
            {
            });
        }
    }
}