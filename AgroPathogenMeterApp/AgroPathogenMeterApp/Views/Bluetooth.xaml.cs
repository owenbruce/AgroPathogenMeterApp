using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;

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
                BtDatabase btDatabase = await DependencyService.Get<IBtControl>().TestConn();

                Name.Text = btDatabase.Name;
                ID.Text = btDatabase.ID.ToString();
                Address.Text = btDatabase.Address;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await DisplayAlert("Error", "Cannot find devices with error: " + ex, "OK");
            }
        }
    }
}