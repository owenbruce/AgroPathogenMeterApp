using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using AgroPathogenMeterApp.iOS;
using AgroPathogenMeterApp.Models;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(BtControl))]
namespace AgroPathogenMeterApp.iOS
{
    class BtControl : IBtControl
    {
        public BtControl() { }
        public static void Init() { }
        public async Task<BtDatabase> TestConn()
        {
            BtDatabase _database = null;
            return _database;
        }
        public async void Connect()
        {

        }
        public string FilePath()
        {
            return "";
        }
    }
}