using AgroPathogenMeterApp.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Run3 : ContentPage
    {
        public Run3()
        {
            InitializeComponent();
        }

        private async void OnRunTestClicked(Object sender, EventArgs e)
        {
            //Setup the standard scan
            ScanDatabase scan = new ScanDatabase
            {
                VoltamType = "Standard",
                Date = DateTime.Now
            };
            await App.Database.SaveScanAsync(scan);
            await Navigation.PushAsync(new DuringRun
            {
            });
        }
    }
}