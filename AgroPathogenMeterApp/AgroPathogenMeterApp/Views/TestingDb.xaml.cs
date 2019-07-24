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
            bool RunningReal = true;
            bool RunningNC = false;
            bool RunningPC = false;

            ScanDatabase scan = new ScanDatabase
            {
                AmountBacteria = 0,
                ConcentrationBacteria = 0,
                Date = DateTime.Now,
                VoltamType = "Real Test"
            };
            await App.Database.SaveScanAsync(scan);

            int fileNum = Convert.ToInt32(fileNumber.Text);
            try
            {
                await DependencyService.Get<IBtControl>().Connect(fileNum, RunningPC, RunningNC, RunningReal);
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
        private async void OnPositiveResultClicked(object sender, EventArgs e)
        {
            bool RunningReal = false;
            bool RunningNC = false;
            bool RunningPC = true;

            ScanDatabase scan = new ScanDatabase
            {
                AmountBacteria = 0,
                ConcentrationBacteria = 0,
                Date = DateTime.Now,
                VoltamType = "Real Test"
            };
            await App.Database.SaveScanAsync(scan);

            int fileNum = Convert.ToInt32(fileNumber.Text);
            try
            {
                await DependencyService.Get<IBtControl>().Connect(fileNum, RunningPC, RunningNC, RunningReal);
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
        private async void OnNegativeResultClicked(object sender, EventArgs e)
        {
            bool RunningReal = false;
            bool RunningNC = true;
            bool RunningPC = false;

            ScanDatabase scan = new ScanDatabase
            {
                AmountBacteria = 0,
                ConcentrationBacteria = 0,
                Date = DateTime.Now,
                VoltamType = "Real Test"
            };
            await App.Database.SaveScanAsync(scan);

            int fileNum = Convert.ToInt32(fileNumber.Text);
            try
            {
                await DependencyService.Get<IBtControl>().Connect(fileNum, RunningPC, RunningNC, RunningReal);
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