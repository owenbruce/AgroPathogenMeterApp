using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PalmSens;
using PalmSens.Comm;
using PalmSens.Data;
using PalmSens.Devices;
//using PalmSens.PSAndroid.Comm;
using PalmSens.Plottables;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Crashes;

namespace AgroPathogenMeterApp.Droid
{
    class BtControl : IBtControl
    {
        Measurement measurement;
        Curve _activeCurve;
        
        public async Task<BtDatabase> TestConn()
        {
            BtDatabase junk = new BtDatabase();
            try
            {
                if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled)
                {

                    foreach (var pairedDevice in BluetoothAdapter.DefaultAdapter.BondedDevices)
                    {

                        BtDatabase btDatabase = new BtDatabase
                        {

                            Name = pairedDevice.Name,
                            Address = pairedDevice.Address
                        };
                        await App.Database2.SaveScanAsync(btDatabase);

                        return btDatabase;
                    }


                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return junk;
            
        }
        /*
        public async void Connect()
        {
            Android.Content.Context context;
            context = (Android.Content.Context)Android.Content.Context.BluetoothService;
            Device[] devices = new Device[0];
            DeviceDiscoverer deviceDiscoverer = new DeviceDiscoverer(context);
            devices = (await deviceDiscoverer.Discover(true, true)).ToArray();
            deviceDiscoverer.Dispose();

            CommManager comm;
            Device device = devices[0];
            try
            {
                device.Open();
                comm = new CommManager(device, 5000);

                comm.ReceiveStatus += Comm_ReceiveStatus;

                comm.BeginMeasurement += Comm_BeginMeasurement;

                comm.BeginReceiveCurve += Comm_BeginReceiveCurve;

                comm.Measure(null);

                //After measurement

                comm.Disconnect();
            }
            catch(Exception ex)
            {
                device.Close();
            }

        }
        */
        private void Comm_ReceiveStatus(object sender, StatusEventArgs e)
        {
            Status status = e.GetStatus();
        }

        private void Comm_BeginMeasurement(object sender, ActiveMeasurement newMeasurement)
        {
            measurement = newMeasurement;
        }

        private void Comm_BeginReceiveCurve(object sender, CurveEventArgs e)
        {
            _activeCurve = e.GetCurve();
            _activeCurve.NewDataAdded += _activeCurve_NewDataAdded;
            _activeCurve.Finished += _activeCurve_Finished;
        }

        private void _activeCurve_NewDataAdded(object sender, ArrayDataAddedEventArgs e)
        {
            int startIndex = e.StartIndex;
            int count = e.Count;
            double[] newData = new double[count];
            (sender as Curve).GetYValues().CopyTo(newData, startIndex);
        }

        private void _activeCurve_Finished(object sender, EventArgs e)
        {
            _activeCurve.NewDataAdded -= _activeCurve_NewDataAdded;
            _activeCurve.Finished -= _activeCurve_Finished;
        }
        public string FilePath()
        {
            return "";
        }
    }
}