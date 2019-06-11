using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllData : ContentPage
    {
        private int i;

        public AllData()
        {
            i = 1;
            InitializeComponent();
            GetDatabase();
        }

        private async void GetDatabase()
        {
            ScanDatabase _database = await App.Database.GetScanAsync(i);
            BindingContext = _database;
        }

        private void OnNextClicked(object sender, EventArgs e)
        {
            try
            {
                i++;
                GetDatabase();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}