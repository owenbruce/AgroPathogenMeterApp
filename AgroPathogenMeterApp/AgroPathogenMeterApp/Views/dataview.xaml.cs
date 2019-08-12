using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
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

        private async void GetContext()   //Get the database which was just scanned and display whether or not the sample is infected
        {
            List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
            ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);
            BindingContext = _database;
        }

        private async void OnSaveDataClicked(object sender, EventArgs e)   //Saves the data once you press the save button
        {
            await Navigation.PopToRootAsync();
        }

        private async void OnViewDataClicked(object sender, EventArgs e)   //Allows the user to view more information about the scan/sample
        {
            await Navigation.PushAsync(new AllData
            {
            });
        }
    }
}