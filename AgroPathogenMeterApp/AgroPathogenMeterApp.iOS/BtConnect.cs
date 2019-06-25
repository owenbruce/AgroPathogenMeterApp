using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgroPathogenMeterApp.Models;
using Foundation;
using UIKit;

namespace AgroPathogenMeterApp.iOS
{
    public class BtConnect : IBtControl
    {
        public string FilePath()
        {
            return null;   //Fix with file location for accessible storage if necessary; otherwise, delete
        }

        public async void connect()
        {

        }

        public async void connect(ScanDatabase _database)
        {

        }
    }
}