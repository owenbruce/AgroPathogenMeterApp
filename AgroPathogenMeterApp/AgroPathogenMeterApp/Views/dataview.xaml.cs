using System;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class dataview : ContentPage   //Menu for viewing data from the last test run/the selected test
    {
        public dataview()
        {
            Analytics.TrackEvent("Dataview opened");
            InitializeComponent();
            GetContext();
        }

        private async void GetContext()
        {
            var allDb = await App.Database.GetScanDatabasesAsync();
            var _database = await App.Database.GetScanAsync(allDb.Count);
            BindingContext = _database;
        }

        private async void OnSaveDataClicked(object sender, EventArgs e)   //Saves the data once you press the save button
        {
            await Navigation.PopToRootAsync();
        }

        private async void OnViewDataClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AllData
            {
            });
        }
    }
}