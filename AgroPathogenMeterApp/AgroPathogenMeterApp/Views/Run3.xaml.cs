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

        private void OnFirstFinishedClicked(object sender, EventArgs e)
        {
            FirstInstr.TextColor = Color.Green;
            FirstButton.IsVisible = false;
            FirstButton.IsEnabled = false;
            SecondInstr.IsVisible = true;
            SecondButton.IsVisible = true;
        }

        private void OnSecondFinishedClicked(object sender, EventArgs e)
        {
            SecondInstr.TextColor = Color.Green;
            SecondButton.IsVisible = false;
            SecondButton.IsEnabled = false;
            ThirdInstr.IsVisible = true;
            ThirdButton.IsVisible = true;
        }

        private void OnThirdFinishedClicked(object sender, EventArgs e)
        {
            ThirdInstr.TextColor = Color.Green;
            ThirdButton.IsVisible = false;
            ThirdButton.IsEnabled = false;
            FourthInstr.IsVisible = true;
            FourthButton.IsVisible = true;
        }

        private async void OnRunTestClicked(object sender, EventArgs e)
        {
            //Setup the standard scan
            ScanDatabase scan = new ScanDatabase
            {
                VoltamType = "Square Wave Voltammetry",
                Date = DateTime.Now,
                StartingPotential = 0.0,
                EndingPotential = -0.6,
                PotentialStep = 0.001,
                Amplitude = 0.025,
                Frequency = 60
            };
            await App.Database.SaveScanAsync(scan);

            DependencyService.Get<IBtControl>().SimpleConnect(1, false, false, true, false);

            await Navigation.PushAsync(new DuringRun
            {
            });
        }

    }
}