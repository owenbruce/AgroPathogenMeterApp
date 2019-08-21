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
            bool RunningDPV = false;

            ScanDatabase scan = new ScanDatabase
            {
                AmountBacteria = 0,
                ConcentrationBacteria = 0,
                Date = DateTime.Now,
                VoltamType = "Linear Voltammetry"
            };
            await App.Database.SaveScanAsync(scan);

            int fileNum = Convert.ToInt32(fileNumber.Text);
            try
            {
                DependencyService.Get<IBtControl>().SimpleConnect(fileNum, RunningPC, RunningNC, RunningReal, RunningDPV);
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
            bool RunningDPV = false;

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
                DependencyService.Get<IBtControl>().SimpleConnect(fileNum, RunningPC, RunningNC, RunningReal, RunningDPV);
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
            bool RunningDPV = false;

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
                DependencyService.Get<IBtControl>().SimpleConnect(fileNum, RunningPC, RunningNC, RunningReal, RunningDPV);
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

        private async void OnDPVResultClicked(object sender, EventArgs e)
        {
            bool RunningReal = false;
            bool RunningNC = false;
            bool RunningPC = false;
            bool RunningDPV = true;

            ScanDatabase scan = new ScanDatabase
            {
                AmountBacteria = 0,
                ConcentrationBacteria = 0,
                Date = DateTime.Now,
                VoltamType = "Differential Pulse Voltammetry",
                EquilTime = 8,
                StartingPotential = -0.5,
                EndingPotential = 0.5,
                PotentialStep = 0.005,
                PotentialPulse = 0.025,
                TimePulse = 0.07,
                ScanRate = 0.025
            };
            await App.Database.SaveScanAsync(scan);

            int fileNum = Convert.ToInt32(fileNumber.Text);
            try
            {
                DependencyService.Get<IBtControl>().SimpleConnect(fileNum, RunningPC, RunningNC, RunningReal, RunningDPV);
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