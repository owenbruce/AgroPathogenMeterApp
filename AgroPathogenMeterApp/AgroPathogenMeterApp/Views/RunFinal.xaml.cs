using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RunFinal : ContentPage
    {
        public RunFinal()
        {
            InitializeComponent();
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
            List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
            ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);
            try
            {
                double startingPotential = _database.StartingPotential;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>
                {
                    {"AllDb Count:", _database.ToString()}
                });
            }
            //Do stuff
            /*
            ScanDatabase _database;
            try
            {
                string boolPath;
                _database = await App.Database.GetScanAsync(allDb.Count());

                if (_database.IsInfected)
                {
                    boolPath = "The sample is infected";
                }
                else
                {
                    boolPath = "The sample isn't infected";
                }

                BindingContext = boolPath;
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
            }

            await Navigation.PushAsync(new dataview
            {
            });
            */
        }
    }
}