using AgroPathogenMeterApp.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class testRunning : ContentPage   //Opens the menu which will be open and show progress while the scan is running and being processed
    {
        public testRunning()
        {
            InitializeComponent();
        }

        /*
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ScanDatabase _database = await App.Database.GetScanAsync(0);
        }
        */

        private async void OnRunTestClicked(object sender, EventArgs e)   //Starts running the test on the APM when the button is tapped
        {
            //Start test on the APM using parameters from _database
            //Once completed, go to finsh test window, and display results
            ScanDatabase _database = await App.Database.GetScanAsync(1);

            await Navigation.PushAsync(new dataview
            {
                BindingContext = _database
            });
        }
    }
}