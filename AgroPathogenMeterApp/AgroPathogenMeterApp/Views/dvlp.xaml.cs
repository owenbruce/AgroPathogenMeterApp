using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class dvlp : ContentPage   //The developer options menu page
    {
        public dvlp()
        {
            InitializeComponent();
        }

        private async void OnManualTestClicked(object sender, EventArgs e)   //Opens the manual tester to choose the test and parameters
        {
            await Navigation.PushAsync(new manTest { });
        }

        private async void OnRawDataClicked(object sender, EventArgs e)   //Opens a file selecter to view raw data from a chosen test
        {
            await Navigation.PushAsync(new rawData { });
        }
    }
}