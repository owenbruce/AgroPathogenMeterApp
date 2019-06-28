using System;
using PalmSens;
using PalmSens.Comm;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgroPathogenMeterApp.Models;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AgroPathogenMeterApp.Droid
{
    class BtConnect : IBtControl
    {
        

        public void connect()
        {
            ClientConnection clientConnection = new ClientConnection();
            CommManager CommManager  = new CommManager(clientConnection);
        }
        public void connect(ScanDatabase _database)
        {
            
        }
        public string FilePath()
        {
            return null;
        }
    }
}