using Android.Content;
using PalmSens.Comm;
using PalmSens.Devices;
using PalmSens.PSAndroid.Comm;
using System;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Droid
{
    internal class BtConnect
    {
        private async void Connector()
        {
            /*
            Context context;
            context = get context
            PalmSens.Devices.Device[] devices = new PalmSens.Devices.Device[0];
            DeviceDiscoverer deviceDiscoverer = new DeviceDiscoverer(context);
            devices = (await deviceDiscoverer.Discover(true, true)).ToArray();
            deviceDiscoverer.Dispose();

            CommManager comm;
            PalmSens.Devices.Device device = devices[0];
            try
            {
                device.Open();
                comm = new CommManager(device, 5000);
            }
            catch (Exception ex)
            {
                device.Close();
                Crashes.TrackError(ex);
            }
            */
        }
    }
}