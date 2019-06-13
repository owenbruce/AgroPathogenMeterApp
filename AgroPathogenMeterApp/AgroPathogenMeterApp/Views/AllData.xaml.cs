using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinUniversity.Infrastructure;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllData : ContentPage
    {
        private int i;
        private ScanDatabase _database;

        public AllData()
        {
            i = 1;
            InitializeComponent();
            GetDatabase();
        }

        private async void GetDatabase()
        {
            if (i > 0)
            {
                var allDB = await App.Database.GetScanDatabasesAsync();
                _database = await App.Database.GetScanAsync(i);
                if (i <= allDB.Count)
                {
                    RefreshGui();
                }
                else
                {
                    i--;
                }
            }
            else
            {
                i++;
            }
        }

        private void RefreshGui()
        {
            var displayStrings = new ObservableDictionary<string, string>();
            try
            {
                if (_database.IsInfected)
                {
                    displayStrings.Add("IsInfected", "The sample is infected");
                    displayStrings.Add("InfectedColor", "Red");
                }
                else
                {
                    displayStrings.Add("IsInfected", "The sample is not infected");
                    displayStrings.Add("InfectedColor", "Green");
                }

                displayStrings.Add("Name", "Name: " + _database.Name);
                displayStrings.Add("ID", "ID: " + _database.ID);
                displayStrings.Add("Date", "Date: " + _database.Date);
                displayStrings.Add("AmountBacteria", "There is " + _database.AmountBacteria + "mol of Bacteria in the urine.");
                displayStrings.Add("ConcentrationBacteria", "There is " + _database.ConcentrationBacteria + "mol/L of Bacteria in the urine.");
                displayStrings.Add("VoltamType", "A " + _database.VoltamType + " scan was run.");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            BindingContext = displayStrings;
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

        private void OnPreviousClicked(object sender, EventArgs e)
        {
            try
            {
                i--;
                GetDatabase();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}