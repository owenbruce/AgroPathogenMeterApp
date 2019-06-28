using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PalmSens;
using PalmSens.Data;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AgroPathogenMeterApp.Droid
{
    class BtControl : IBtControl
    {
        public void Connect()
        {
            Measurement measurement = new Measurement();
        }
    }
}