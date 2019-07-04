using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PalmSens;
using PalmSens.Comm;
using PalmSens.Data;
using PalmSens.Devices;
using PalmSens.PSAndroid.Comm;
using PalmSens.Plottables;
using PalmSens.Techniques;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Crashes;
using AgroPathogenMeterApp.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(BtControl))]

namespace AgroPathogenMeterApp.Droid
{
    class BtControl : IBtControl
    {
        Measurement measurement;
        Curve _activeCurve;
        
        public BtControl() { }
        public static void Init() { }
        public async Task<BtDatabase> TestConn()   //Test the connection to the APM
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

                Method m = await RunScan();
                try
                {
                    comm.Measure(m);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                //After measurement

                comm.Disconnect();
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
                device.Close();
            }

        }
        
        //Below allows for starting the necessary measurements on the APM
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

        //Below runs the necessary scan on the APM
        private async Task<Method> RunScan()
        {
            var allDb = await App.Database.GetScanDatabasesAsync();
            var _database = await App.Database.GetScanAsync(allDb.Count);
            var instance = new ScanParams();

            switch (_database.VoltamType)
            {
                case "Linear Voltammetry":
                    LinearSweep linSweep = instance.LinSweep(_database);

                    linSweep.Ranging.StartCurrentRange = new CurrentRange(5);
                    linSweep.Ranging.MaximumCurrentRange = new CurrentRange(6);
                    linSweep.Ranging.MaximumCurrentRange = new CurrentRange(3);

                    return linSweep;

                case "Cyclic Voltammetry":
                    CyclicVoltammetry cVoltammetry = instance.CV(_database);

                    cVoltammetry.Ranging.StartCurrentRange = new CurrentRange(5);
                    cVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(6);
                    cVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(3);

                    return cVoltammetry;

                case "Square Wave Voltammetry":
                    SquareWave squareWave = instance.SWV(_database);

                    squareWave.Ranging.StartCurrentRange = new CurrentRange(5);
                    squareWave.Ranging.MaximumCurrentRange = new CurrentRange(6);
                    squareWave.Ranging.MaximumCurrentRange = new CurrentRange(3);

                    return squareWave;

                case "Alternating Current Voltammetry":
                    ACVoltammetry acVoltammetry = instance.ACV(_database);

                    acVoltammetry.Ranging.StartCurrentRange = new CurrentRange(5);
                    acVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(6);
                    acVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(3);

                    return acVoltammetry;

                default:
                    //Add code to notify user that something has gone wrong and needs to be fixed

                    return null;
            }
        }


        public string FilePath()
        {
            return "";
        }
    }
}