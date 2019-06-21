using AgroPathogenMeterApp.Data;
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
        private ScanDatabase Scan;
        private List<ScanDatabase> allDB;
        private int i;

        public AllData()
        {
            i = 1;
            SetDatabaseList();
            InitializeComponent();
            GetDatabase();
        }
        private async void SetDatabaseList()
        {
            allDB = await App.Database.GetScanDatabasesAsync();
        }
        private async void GetDatabase()
        {
            if (i > 0)
            {
                Scan = await App.Database.GetScanAsync(i);
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
                if (Scan.IsInfected)
                {
                    displayStrings.Add("IsInfected", "The sample is infected");
                    displayStrings.Add("InfectedColor", "Red");
                }
                else
                {
                    displayStrings.Add("IsInfected", "The sample is not infected");
                    displayStrings.Add("InfectedColor", "Green");
                }

                displayStrings.Add("Name", "Name: " + Scan.Name);
                displayStrings.Add("ID", "ID: " + i);
                displayStrings.Add("Date", "Date: " + Scan.Date);
                displayStrings.Add("AmountBacteria", "There is " + Scan.AmountBacteria + "cfu of Bacteria in the urine.");
                displayStrings.Add("ConcentrationBacteria", "There is " + Scan.ConcentrationBacteria + "cfu/ml of Bacteria in the urine.");
                displayStrings.Add("VoltamType", "A " + Scan.VoltamType + " scan was run.");

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            BindingContext = displayStrings;
        }
    }
}