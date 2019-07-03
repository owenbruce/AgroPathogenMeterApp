using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgroPathogenMeterApp.Models
{
    public class BtDatabase   //Holds information for connecting to the APM
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
    }
}
