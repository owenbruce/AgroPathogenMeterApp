using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Analytics;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestingDb : ContentPage
    {
        public TestingDb()
        {
            Analytics.TrackEvent("Testing Database opened");
            InitializeComponent();
        }

        private async void OnViewResultClicked(object sender, EventArgs e)
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

            await Navigation.PushAsync(new AllData
            {
            });
        }
    }
}