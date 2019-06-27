using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RunFinal : ContentPage
    {
        private String isInfect;
        public RunFinal()
        {
            Analytics.TrackEvent("Scan completed");
            IsInfected();
            InitializeComponent();
        }

        private async void IsInfected()
        {
            var allDb = await App.Database.GetScanDatabasesAsync();
            var _database = await App.Database.GetScanAsync(allDb.Count);
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
        private async void OnMoreInfoClicked(object sender, EventArgs e)
        {
            //Do other stuff
            await Navigation.PushAsync(new AllData
            {
            });
        }

        private async void OnSaveResultClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new dataview
            {
            });
            
        }
    }
}