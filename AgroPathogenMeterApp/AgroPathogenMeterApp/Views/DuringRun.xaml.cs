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
        public DuringRun()
        {
            Analytics.TrackEvent("DuringRun Opened");
            InitializeComponent();
            //ContinueRun();
        }

        private async void ContinueRun()
        {
            isVisible = await Visible();
        }

        private async Task<bool> Visible()
        {
            Thread.Sleep(5000);
            return await Task.FromResult<bool>(true);
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RunFinal
            {
            });
        }
    }
}