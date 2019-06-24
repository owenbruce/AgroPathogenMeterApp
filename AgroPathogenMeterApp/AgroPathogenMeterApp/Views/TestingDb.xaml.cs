using AgroPathogenMeterApp.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestingDb : ContentPage
    {
        public TestingDb()
        {
            InitializeComponent();
        }

        private async void OnViewResultClicked(object sender, EventArgs e)
        {
            ScanDatabase scan = new ScanDatabase
            {
                AmountBacteria = 0,
                ConcentrationBacteria = 0,
                Date = DateTime.Now,
                IsInfected = false,
                VoltamType = Entry1.Text
            };
            await App.Database.SaveScanAsync(scan);

            await Navigation.PushAsync(new AllData
            {
            });
        }
    }
}