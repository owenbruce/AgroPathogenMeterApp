using Android.Content;
using PalmSens;
using PalmSens.Comm;
using PalmSens.Devices;
using PalmSens.PSAndroid.Comm;
using System;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Droid
{
    internal class DeviceHandler
    {
        internal DeviceHandler()
        {
        }

        internal Context Context = Android.App.Application.Context;
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
        internal async Task<Device[]> ScanDevicesAsync(int timeOut = 20000)
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
                throw new ArgumentException($"An error occured while attempting to scan for connected devices. {ex.Message}");
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
        internal async Task<CommManager> Connect(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("The specified device cannot be null.");
            CommManager comm = null;

            await new SynchronizationContextRemover();

            try
            {
                await device.OpenAsync(); //Open the device to allow a connection
                comm = await CommManager.CommManagerAsync(device); //Connect to the selected device
            }
            catch (Exception ex)
            {
                device.Close();
                throw new Exception($"Could not connect to the specified device. {ex.Message}");
            }

            return comm;
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
        [Obsolete("Compatible with SDKs 5.4 and earlier. Please use asynchronous functions, as development of synchronous functions will be fased out")]
        internal CommManager ConnectBC(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("The specified device cannot be null.");
            CommManager comm = null;

            try
            {
                device.Open(); //Open the device to allow a connection
                comm = new CommManager(device); //Connect to the selected device
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