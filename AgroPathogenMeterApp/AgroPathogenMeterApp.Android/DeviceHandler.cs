using Android.Content;
using Microsoft.AppCenter.Crashes;
using PalmSens.Comm;
using PalmSens.Devices;
using PalmSens.PSAndroid.Comm;
using System;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Droid
{
    internal class DeviceHandler
    {
        public DeviceHandler()
        {
        }

        internal Context Context = Android.App.Application.Context;
        internal DeviceDiscoverer _deviceDiscoverer;

        internal bool EnableBluetooth = true;
        internal bool EnableUSB = false;

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
                Crashes.TrackError(ex);
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
            {
                throw new ArgumentNullException("The specified device cannot be null.");
            }

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
            {
                throw new ArgumentNullException("The specified CommManager cannot be null.");
            }

            comm.Disconnect();
            comm = null;
        }
    }
}