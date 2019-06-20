using Microsoft.VisualStudio.TestPlatform.Utilities.Helpers;
using SQLite;
using System;
using Xamarin.Forms;


namespace AgroPathogenMeterApp.Models
{
    public class ScanDatabase   //Current SQLite database which contains all of the necessary information for the scan parameters, results, and identifying information
    {
        [PrimaryKey, AutoIncrement]
        public bool IsInfected { get; set; } = false;

        public DateTime Date { get; set; }
        public double ACPotential { get; set; } = 0.0;
        public double AmountBacteria { get; set; } = 0.0;
        public double Amplitude { get; set; } = 0.0;
        public double AppliedPotential { get; set; } = 0.0;
        public double ConcentrationBacteria { get; set; } = 0.0;
        public double EndingPotential { get; set; } = 0.0;
        public double Frequency { get; set; } = 0.0;
        public double NegativeVertex { get; set; } = 0.0;
        public double PositiveVertex { get; set; } = 0.0;
        public double PotentialStep { get; set; } = 0.0;
        public double RunTime { get; set; } = 0.0;
        public double ScanRate { get; set; } = 0.0;
        public double StartingPotential { get; set; } = 0.0;
        public double TimeInterval { get; set; } = 0.0;
        public int ID { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Number { get; set; } = "";
        public string VoltamType { get; set; } = "";
    }
}