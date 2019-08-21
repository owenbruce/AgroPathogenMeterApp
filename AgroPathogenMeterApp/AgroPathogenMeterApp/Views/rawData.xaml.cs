using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinUniversity.Infrastructure;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RawData : ContentPage   //Allows for developers to view the raw data
    {
        private ScanDatabase Scan;
        private List<ScanDatabase> allDb;
        private int i;

        public RawData()   //Allow a developer to view the raw data
        {
            Analytics.TrackEvent("Raw Data Opened");
            SetDatabaseList();
            i = 1;
            InitializeComponent();
            GetDatabase();
        }

        private async void SetDatabaseList()
        {
            allDb = await App.Database.GetScanDatabasesAsync();
        }

        private async void GetDatabase()
        {
            if (i > 0)
            {
                Scan = await App.Database.GetScanAsync(i);
                if (i <= allDb.Count)
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
            ObservableDictionary<string, string> displayStrings = new ObservableDictionary<string, string>
            {
                { "ID", "ID: " + Scan.ID.ToString() },
                { "IsInfected", "IsInfected: " + Scan.IsInfected.ToString() },
                { "Date", "Date: " + Scan.Date.ToString() },
                { "ACPotential", "ACPotential: " + Scan.ACPotential.ToString() },
                { "AmountBacteria", "AmountBacteria: " + Scan.AmountBacteria.ToString() },
                { "Amplitude", "Amplitude: " + Scan.Amplitude.ToString() },
                { "ConcentrationBacteria", "ConcentrationBacteria: " + Scan.ConcentrationBacteria.ToString() },
                { "EndingPotential", "EndingPotential: " + Scan.EndingPotential.ToString() },
                { "EquilTime", "EquilTime: " + Scan.EquilTime.ToString() },
                { "Frequency", "Frequency: " + Scan.Frequency.ToString() },
                { "NegativeVertex", "NegativeVertex: " + Scan.NegativeVertex.ToString() },
                { "PeakVoltage", "PeakVoltage: " + Scan.PeakVoltage.ToString() },
                { "PositiveVertex", "PositiveVertex: " + Scan.PositiveVertex.ToString() },
                { "PotentialPulse", "PotentialPulse" + Scan.PotentialPulse.ToString() },
                { "PotentialStep", "PotentialStep: " + Scan.PotentialStep.ToString() },
                { "ScanRate", "ScanRate: " + Scan.ScanRate.ToString() },
                { "StartingPotential", "StartingPotential: " + Scan.StartingPotential.ToString() },
                { "TimePulse", "TimePulse: " + Scan.TimePulse.ToString() },
                { "Name", "Name: " + Scan.Name },
                { "VotamType", "VoltamType: " + Scan.VoltamType }
            };
            BindingContext = displayStrings;
        }
    }
}