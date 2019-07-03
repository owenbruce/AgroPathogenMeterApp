using System;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DuringRun : ContentPage
    {
        bool isVisible = false;
        public DuringRun()   //Display progress of the scan while the scan is taking place
        {
            Analytics.TrackEvent("DuringRun Opened");
            InitializeComponent();
            //ContinueRun();
        }

        private async void ContinueRun()   //Make the next button invisible so the user cannot proceed until the scan has been completed
        {
            isVisible = await Visible();
        }

        private async Task<bool> Visible()
        {
            Thread.Sleep(5000);
            return await Task.FromResult<bool>(true);
        }

        private async void OnContinueClicked(object sender, EventArgs e)   //Continue onto dataview to see the results of the scan
        {
            await Navigation.PushAsync(new RunFinal
            {
            });
        }
    }
}