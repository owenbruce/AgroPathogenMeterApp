using AgroPathogenMeterApp.Models;
using Android.Bluetooth;
using Android.Content;

namespace AgroPathogenMeterApp.Droid
{
    public class BluetoothDeviceReceiver : BroadcastReceiver   //Allow for the phone to connect to the APM
    {
        public override async void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;

            if (action != BluetoothDevice.ActionFound)
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

                await App.Database2.SaveScanAsync(btDatabase);
            }
        }
    }
}