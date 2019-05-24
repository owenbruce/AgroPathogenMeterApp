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
        //public DateTime Date { get; set; }
    }
}