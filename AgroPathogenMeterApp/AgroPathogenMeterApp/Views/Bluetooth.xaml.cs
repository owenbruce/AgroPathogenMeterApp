using Microsoft.AppCenter.Analytics;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinUniversity.Infrastructure;
using AgroPathogenMeterApp.Models;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Bluetooth : ContentPage
    {
        public Bluetooth()
        {
            Analytics.TrackEvent("Bluetooth Opened");
            InitializeComponent();
        }

        private async void OnBtClicked(object sender, EventArgs e)
        {
            try
            {
                BtDatabase btDatabase = await DependencyService.Get<IBtControl>().TestConn();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Cannot find devices with error: " + ex, "OK");
            }


            ObservableDictionary<string, string> displayString = new ObservableDictionary<string, string>();

        }
    }
}