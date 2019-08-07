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
                Date = DateTime.Now,

                StartingPotential = 0.0,
                EndingPotential = -0.6,
                PotentialStep = 0.001,
                Amplitude = 0.025,
                Frequency = 60
            };
            await App.Database.SaveScanAsync(scan);

            DependencyService.Get<IBtControl>().Connect(1, false, false, true, false);

            await Navigation.PushAsync(new DuringRun
            {
            });
        }
    }
}