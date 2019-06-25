using System;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DuringRun : ContentPage
    {
        public DuringRun()
        {
            Analytics.TrackEvent("DuringRun Opened");
            InitializeComponent();
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RunFinal
            {
            });
        }
    }
}