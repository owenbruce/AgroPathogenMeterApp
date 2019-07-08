tem;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PalmSens.Devices;
using PalmSens.Comm;
using Android.Bluetooth;
using System.Threading.Tasks;
using PalmSens.PSAndroid.Comm;

namespace PalmSens.Core.Simplified.Android
{
    class DeviceHandler
    {
        internal DeviceHandler()
        {
        }

        internal Context Context;
        internal DeviceDiscoverer _deviceDiscoverer;

        internal bool EnableBluetooth = true;
        internal bool EnableUSB = true;

        /// <summary>
        /// Scans for connected devices.
        /// </summary>
        /// <param name="timeOut">Discovery time out in milliseconds.</param>
        /// <returns>
        /// Returns an array of connected devices
        /// </returns>
        /// <exception cref="System.ArgumentException">An error occured while attempting to scan for connected devices.</exception>
        internal async Task<Device[]> ScanDevices(int timeOut = 20000)
        {
            Device[] devices = new Device[0];

            try //Attempt to find connected palmsens/emstat devices
            {
                _deviceDiscoverer = new DeviceDiscoverer(Context);
                devices = (await _deviceDiscoverer.Discover(EnableUSB, EnableBluetooth, timeOut)).ToArray();
                _deviceDiscoverer.Dispose();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"An error occured while attempting to scan for connected devices.");
            }
            return devices;
        }

        /// <summary>
        /// Connects to the specified device and returns its CommManager.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns>
        /// The CommManager of the device or null
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The specified device cannot be null.</exception>
        /// <exception cref="System.Exception">Could not connect to the specified device.</exception>
        internal CommManager Connect(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("The specified device cannot be null.");
            CommManager comm = null;

            try
            {
                device.Open(); //Open the device to allow a connection
                comm = new CommManager(device, 3000); //Connect to the selected device
            }
            catch (Exception ex)
            {
                device.Close();
                throw new Exception($"Could not connect to the specified device. {ex.Message}");
            }

            return comm;
        }

        /// <summary>
        /// Disconnects the device using its CommManager.
        /// </summary>
        /// <param name="comm">The device's CommManager.</param>
        /// <exception cref="System.ArgumentNullException">The specified CommManager cannot be null.</exception>
        internal void Disconnect(CommManager comm)
        {
            if (comm == null)
                throw new ArgumentNullException("The specified CommManager cannot be null.");
            comm.Disconnect();
            comm = null;
        }
    }
}