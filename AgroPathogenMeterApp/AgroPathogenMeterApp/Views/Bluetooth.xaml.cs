using Microsoft.AppCenter.Analytics;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        private async void OnBtClicked(Object sender, EventArgs e)
        {
            try
            {
                //DependencyService.Get<IBtControl>().TestConn();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Cannot find devices with error: " + ex, "OK");
            }
        }
    }
}