using SQLite;
using System;

namespace AgroPathogenMeterApp.Models
{
    public class ScanDatabase   //Current SQLite database which contains all of the necessary information for the scan parameters, results, and identifying information
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Number { get; set; }
        public bool IsInfected { get; set; }
        public double AmountBacteria { get; set; }
        public double ConcentrationBacteria { get; set; }
        public DateTime Date { get; set; }
        public string VoltamType { get; set; }
        public double StartingPotential { get; set; }
        public double EndingPotential { get; set; }
        public double PotentialStep { get; set; }
        public double Amplitude { get; set; }
        public double Frequency { get; set; }
        public double NegativeVertex { get; set; }
        public double PositiveVertex { get; set; }
        public double ScanRate { get; set; }
        public double ACPotential { get; set; }
    }
}