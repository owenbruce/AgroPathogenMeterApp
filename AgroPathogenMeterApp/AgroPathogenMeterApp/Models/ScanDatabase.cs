using SQLite;
using System;

namespace AgroPathogenMeterApp.Models
{
    public class ScanDatabase
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public bool Infected { get; set; }
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