using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Crashes;
using Microsoft.VisualStudio.TestPlatform.Utilities.Helpers;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinUniversity.Infrastructure;

[assembly: Dependency(typeof(FileHelper))]

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllData : ContentPage
    {
        private ScanDatabase _database;
        private List<ScanDatabase> allDB;
        private int i;

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
                allDB = await App.Database.GetScanDatabasesAsync();
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

        private void RefreshGui()
        {
            ObservableDictionary<string, string> displayStrings = new ObservableDictionary<string, string>();
            try
            {
                if (_database == null)
                {
                    displayStrings.Add("IsInfected", "Database is null");
                    displayStrings.Add("Name", _database.ID.ToString());
                }
                else
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

                    displayStrings.Add("Name", "Name: " + _database.Name.ToString());
                    displayStrings.Add("ID", "ID: " + i.ToString());
                    displayStrings.Add("Date", "Date: " + _database.Date.ToString());
                    displayStrings.Add("AmountBacteria", "There is " + _database.AmountBacteria.ToString() + "cfu of Bacteria in the urine.");
                    displayStrings.Add("ConcentrationBacteria", "There is " + _database.ConcentrationBacteria.ToString() + "cfu/ml of Bacteria in the urine.");
                    displayStrings.Add("VoltamType", "A " + _database.VoltamType + " scan was run.");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            BindingContext = displayStrings;
        }
    }
}