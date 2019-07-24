using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Analytics;
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

        public AllData()   //Allow the user to view more specific information about scan such as extimated amount of bacteria, date, and more
        {
            Analytics.TrackEvent("AllData opened");
            SetDatabaseList();
            i = 1;
            InitializeComponent();
            GetDatabase();
        }

        private async void SetDatabaseList()   //Set a list to contain all databases stored currently in the file
        {
            allDB = await App.Database.GetScanDatabasesAsync();
        }

        private async void GetDatabase()   //Get a specific database depending on where you are in the list of databases
        {
            if (i > 0)
            {
                Scan = await App.Database.GetScanAsync(i);
                if (i <= allDB.Count)
                {
                    RefreshGui();
                }
                else    //If you try and go to the next database and it doesn't exist, don't change and reset i to it's previous value
                {
                    i--;
                }
            }
            else   //If you try and go to the previous database and it doesn't exist, don't change and reset i to it's previous value
            {
                i++;
            }
        }

        private void OnNextClicked(object sender, EventArgs e)   //Allow the user to view the next database in the list
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

        private void OnPreviousClicked(object sender, EventArgs e)   //Allow the user to view the previous database in the list
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
            ObservableDictionary<string, string> displayStrings = new ObservableDictionary<string, string>();   //Initialize a observable dictionary to allow the xaml file to grab information from the current database and display it
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
                displayStrings.Add("AmountBacteria", "The peak voltage was " + Scan.PeakVoltage + "V");
                displayStrings.Add("ConcentrationBacteria", "There is approximately " + Scan.ConcentrationBacteria + " cfu/ml of Bacteria in the urine.");
                displayStrings.Add("VoltamType", "A " + Scan.VoltamType + " scan was run.");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            BindingContext = displayStrings;   //Set the binding context to the observable dictionary
        }
    }
}