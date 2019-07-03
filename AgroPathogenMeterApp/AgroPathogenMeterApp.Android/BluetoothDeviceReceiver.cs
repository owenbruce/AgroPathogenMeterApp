using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AgroPathogenMeterApp.Models;

namespace AgroPathogenMeterApp.Droid
{
    public class BluetoothDeviceReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;

            if (action!= BluetoothDevice.ActionFound)
            {
                return;
            }

            var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

            if (device.BondState != Bond.Bonded)
            {
                //Add new bt device database
                BtDatabase btDatabase = new BtDatabase
                {
                    Name = device.Name,
                    Address = device.Address
                };

                //await App.Database2.SaveScanAsync(btDatabase);
            }
        }
    }
}