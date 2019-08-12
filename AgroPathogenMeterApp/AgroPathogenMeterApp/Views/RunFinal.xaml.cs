using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RunFinal : ContentPage
    {
        private String isInfect;

        public RunFinal()   //Show the final information about the scan and allow to view more information if wanted
        {
            Analytics.TrackEvent("Scan completed");
            IsInfected();
            InitializeComponent();
        }

        private async void IsInfected()   //Displays whether or not the sample is infected
        {
            List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
            ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);
            if (_database.IsInfected)
            {
                isInfect = "The sample is infected";
            }
            else
            {
                isInfect = "The sample is not infected";
            }
            boolPath.Text = isInfect;
        }

        private async void OnMoreInfoClicked(object sender, EventArgs e)   //Opens the screen with more information
        {
            //Do other stuff
            await Navigation.PushAsync(new AllData
            {
            });
        }

        private async void OnSaveResultClicked(object sender, EventArgs e)   //"Saves" the result into the database, potentially have this store to a server
        {
            await Navigation.PushAsync(new dataview
            {
            });
        }
    }
}