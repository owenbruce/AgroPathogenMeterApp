using Microsoft.AppCenter.Analytics;
using System;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DuringRun : ContentPage
    {
        private bool isEnabled = false;

        public DuringRun()   //Display progress of the scan while the scan is taking place
        {
            Analytics.TrackEvent("DuringRun Opened");
            InitializeComponent();
            ContinueRun();
        }

        private void ContinueRun()   //Make the next button invisible so the user cannot proceed until the scan has been completed
        {
            for (double i = 0; i < 1; i += 0.001)
            {
                progressBar.Progress = i;

                Thread.Sleep(31);
            }

            isEnabled = true;
        }

        private async void OnContinueClicked(object sender, EventArgs e)   //Continue onto dataview to see the results of the scan
        {
            await Navigation.PushAsync(new RunFinal
            {
            });
        }
    }
}