using AgroPathogenMeterApp.Droid;
using AgroPathogenMeterApp.Models;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.Res;
using Android.Util;
using Microsoft.AppCenter.Crashes;
using PalmSens;
using PalmSens.Analysis;
using PalmSens.Comm;
using PalmSens.Data;
using PalmSens.Devices;
using PalmSens.Plottables;
using PalmSens.Techniques;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(BtControl_Android))]

namespace AgroPathogenMeterApp.Droid
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class BtControl_Android : IBtControl
    {
        private Curve _activeCurve;
        private SimpleCurve _activeSimpleCurve;
        private Measurement measurement;
        public void _activeCurve_Finished(object sender, EventArgs e)
        {
            _activeCurve.NewDataAdded -= _activeCurve_NewDataAdded;
            _activeCurve.Finished -= _activeCurve_Finished;
        }

        public void _activeCurve_NewDataAdded(object sender, ArrayDataAddedEventArgs e)
        {
            int startIndex = e.StartIndex;
            int count = e.Count;
            double[] newData = new double[count];
            (sender as Curve).GetYValues().CopyTo(newData, startIndex);
        }

        public void _activeSimpleCurve_CurveFinished(object sender, EventArgs e)
        {
            _activeSimpleCurve.NewDataAdded -= _activeCurve_NewDataAdded;
            _activeSimpleCurve.CurveFinished -= _activeSimpleCurve_CurveFinished;
        }

        public void _activeSimpleCurve_NewDataAdded(object sender, ArrayDataAddedEventArgs e)
        {
            int startIndex = e.StartIndex;
            int count = e.Count;
            double[] newData = new double[count];
            (sender as SimpleCurve).YAxisValues.CopyTo(newData, startIndex);
        }

        public async Task<BtDatabase> AsyncTask(BluetoothDevice pairedDevice)
        {
            BtDatabase btDatabase = new BtDatabase
            {
                Name = pairedDevice.Name,
                Address = pairedDevice.Address
            };
            await App.Database2.SaveScanAsync(btDatabase);

            return btDatabase;
        }

        public void Comm_BeginMeasurement(object sender, ActiveMeasurement newMeasurement)
        {
            measurement = newMeasurement;
        }

        public void Comm_BeginReceiveCurve(object sender, CurveEventArgs e)
        {
            _activeCurve = e.GetCurve();
            _activeCurve.NewDataAdded += _activeCurve_NewDataAdded;
            _activeCurve.Finished += _activeCurve_Finished;
        }

        //Below allows for starting the necessary measurements on the APM
        public void Comm_ReceiveStatus(object sender, StatusEventArgs e)
        {
            Status status = e.GetStatus();
        }

        public async Task Connect(bool simple)
        {
            SimpleConnect();
            return;

            /*
            Context context = Application.Context;
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
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                device.Close();
            }
            */
        }

        public string FilePath()
        {
            return "";
        }

        public void PsCommSimpleAndroid_MeasurementEnded(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void PsCommSimpleAndroid_MeasurementStarted(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void PsCommSimpleAndroid_SimpleCurveStartReceivingData(object sender, SimpleCurve activeSimpleCurve)
        {
            _activeSimpleCurve = activeSimpleCurve;
            _activeSimpleCurve.NewDataAdded += _activeSimpleCurve_NewDataAdded;
            _activeSimpleCurve.CurveFinished += _activeSimpleCurve_CurveFinished;
        }

        //Below runs the necessary scan on the APM
        public async Task<Method> RunScan()
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

        public async void SimpleConnect()
        {
            //Below sets which option the code will execute

            bool RunningPC = true;
            bool RunningNC = false;
            bool RunningReal = false;

            if (RunningReal)
            {
                Context context = Application.Context;
                IAttributeSet attributeSet = null;
                PSCommSimpleAndroid psCommSimpleAndroid = new PSCommSimpleAndroid(context, attributeSet);
                Device[] devices = await psCommSimpleAndroid.GetConnectedDevices();

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        psCommSimpleAndroid.Connect(devices[i]);
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                }
                psCommSimpleAndroid.MeasurementStarted += PsCommSimpleAndroid_MeasurementStarted;
                psCommSimpleAndroid.MeasurementEnded += PsCommSimpleAndroid_MeasurementEnded;
                psCommSimpleAndroid.SimpleCurveStartReceivingData += PsCommSimpleAndroid_SimpleCurveStartReceivingData;
                var runScan = await RunScan();
                SimpleMeasurement activeSimpleMeasurement = psCommSimpleAndroid.Measure(runScan);
                SimpleLoadSaveFunctions.SaveMeasurement(activeSimpleMeasurement, null);

                List<SimpleCurve> simpleCurves = activeSimpleMeasurement.SimpleCurveCollection;

                SimpleCurve subtractedCurve = simpleCurves[0].Subtract(simpleCurves[1]);    //Note, replace simpleCurves[1] w/ the standard blank curve

                subtractedCurve.DetectPeaks();
                PeakList peakList = subtractedCurve.Peaks;
                Peak mainPeak = peakList[0];
                double peakHeight = mainPeak.PeakValue;

                var allDb = await App.Database.GetScanDatabasesAsync();
                var _database = await App.Database.GetScanAsync(allDb.Count);

                if (peakHeight <= -0.001)
                {
                    _database.IsInfected = true;
                }
                else
                {
                    _database.IsInfected = false;
                }
            }
            else if (RunningNC || RunningPC)
            {
                SimpleMeasurement baseline;
                AssetManager assetManager = Application.Context.Assets;
                using (StreamReader sr = new StreamReader(assetManager.Open("2525AfterProbe.pssession")))
                    baseline = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

                List<SimpleCurve> baselineCurves = baseline.SimpleCurveCollection;

                if (RunningPC)
                {
                    SimpleMeasurement positiveControl;
                    using (StreamReader sr = new StreamReader(assetManager.Open("2525AfterTarget.pssession")))
                        positiveControl = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

                    List<SimpleCurve> positiveCurves = positiveControl.SimpleCurveCollection;

                    SimpleCurve subtractedCurve = positiveCurves[0].Subtract(baselineCurves[0]);

                    subtractedCurve.DetectPeaks();
                    PeakList positivePeakList = subtractedCurve.Peaks;
                    Peak positivePeak = positivePeakList[0];
                    double positivePeakHeight = positivePeak.PeakValue;

                    var allDb = await App.Database.GetScanDatabasesAsync();
                    var _database = await App.Database.GetScanAsync(allDb.Count);

                    if (positivePeakHeight <= -0.001)
                    {
                        _database.IsInfected = true;
                    }
                    else
                    {
                        _database.IsInfected = false;
                    }
                }
                else if (RunningNC)
                {
                    SimpleMeasurement negativeControl;
                    using (StreamReader sr = new StreamReader(assetManager.Open("2525AfterProbe.pssession")))
                        negativeControl = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

                    List<SimpleCurve> negativeCurves = negativeControl.SimpleCurveCollection;

                    SimpleCurve subtractedCurve = negativeCurves[0].Subtract(baselineCurves[0]);

                    subtractedCurve.DetectPeaks();
                    PeakList negativePeakList = subtractedCurve.Peaks;
                    Peak negativePeak = negativePeakList[0];
                    double negativePeakHeight = negativePeak.PeakValue;

                    var allDb = await App.Database.GetScanDatabasesAsync();
                    var _database = await App.Database.GetScanAsync(allDb.Count);

                    if (negativePeakHeight <= -0.001)
                    {
                        _database.IsInfected = true;
                    }
                    else
                    {
                        _database.IsInfected = false;
                    }
                }
            }

        }

        public async Task<BtDatabase> TestConn()   //Test the connection to the APM
        {
            BtDatabase junk = new BtDatabase();
            try
            {
                if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled)
                {
                    foreach (var pairedDevice in BluetoothAdapter.DefaultAdapter.BondedDevices)
                    {
                        return await AsyncTask(pairedDevice);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return junk;
        }
    }
}