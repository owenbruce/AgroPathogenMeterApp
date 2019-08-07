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
        private bool MeasurementRunning;
        private SimpleMeasurement activeSimpleMeasurement;
        private List<SimpleCurve> baselineCurves;

        #region Flags

        protected virtual void _activeCurve_Finished(object sender, EventArgs e)
        {
            _activeCurve.NewDataAdded -= _activeCurve_NewDataAdded;
            _activeCurve.Finished -= _activeCurve_Finished;
        }

        protected virtual void _activeCurve_NewDataAdded(object sender, ArrayDataAddedEventArgs e)
        {
            int startIndex = e.StartIndex;
            int count = e.Count;
            double[] newData = new double[count];
            (sender as Curve).GetYValues().CopyTo(newData, startIndex);
        }

        protected virtual void _activeSimpleCurve_CurveFinished(object sender, EventArgs e)
        {
            _activeSimpleCurve.NewDataAdded -= _activeCurve_NewDataAdded;
            _activeSimpleCurve.CurveFinished -= _activeSimpleCurve_CurveFinished;
        }

        protected virtual void _activeSimpleCurve_NewDataAdded(object sender, ArrayDataAddedEventArgs e)
        {
            int startIndex = e.StartIndex;
            int count = e.Count;
            double[] newData = new double[count];
            (sender as SimpleCurve).YAxisValues.CopyTo(newData, startIndex);
        }

        protected virtual async Task<BtDatabase> AsyncTask(BluetoothDevice pairedDevice)
        {
            BtDatabase btDatabase = new BtDatabase
            {
                Name = pairedDevice.Name,
                Address = pairedDevice.Address
            };
            await App.Database2.SaveScanAsync(btDatabase);

            return btDatabase;
        }

        protected virtual void Comm_BeginMeasurement(object sender, ActiveMeasurement newMeasurement)
        {
            measurement = newMeasurement;
        }

        protected virtual void Comm_BeginReceiveCurve(object sender, CurveEventArgs e)
        {
            _activeCurve = e.GetCurve();
            _activeCurve.NewDataAdded += _activeCurve_NewDataAdded;
            _activeCurve.Finished += _activeCurve_Finished;
        }

        //Below allows for starting the necessary measurements on the APM
        protected virtual void Comm_ReceiveStatus(object sender, StatusEventArgs e)
        {
            Status status = e.GetStatus();
        }

        protected virtual async void PsCommSimpleAndroid_MeasurementEnded(object sender, EventArgs e)
        {
            MeasurementRunning = false;
        }

        protected virtual void PsCommSimpleAndroid_MeasurementStarted(object sender, EventArgs e)
        {
            MeasurementRunning = true;
        }

        protected virtual void PsCommSimpleAndroid_SimpleCurveStartReceivingData(object sender, SimpleCurve activeSimpleCurve)
        {
            _activeSimpleCurve = activeSimpleCurve;
            _activeSimpleCurve.NewDataAdded += _activeSimpleCurve_NewDataAdded;
            _activeSimpleCurve.CurveFinished += _activeSimpleCurve_CurveFinished;
        }

        #endregion Flags

        public async Task Connect(int fileNum, bool RunningPC, bool RunningNC, bool RunningReal, bool RunningDPV)
        {
            SimpleConnect(fileNum, RunningPC, RunningNC, RunningReal, RunningDPV);
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

        //Below runs the necessary scan on the APM
        public async Task<DifferentialPulse> RunScan()
        {
            var allDb = await App.Database.GetScanDatabasesAsync();
            var _database = await App.Database.GetScanAsync(allDb.Count);
            var instance = new ScanParams();

            switch (_database.VoltamType)
            {
                case "Alternating Current Voltammetry":
                    ACVoltammetry acVoltammetry = instance.ACV(_database);

                    acVoltammetry.Ranging.StartCurrentRange = new CurrentRange(5);
                    acVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(6);
                    acVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(3);

                    return null;

                case "Cyclic Voltammetry":
                    CyclicVoltammetry cVoltammetry = instance.CV(_database);

                    cVoltammetry.Ranging.StartCurrentRange = new CurrentRange(5);
                    cVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(6);
                    cVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(3);

                    return null;

                case "Differential Pulse Voltammetry":
                    DifferentialPulse differentialPulse = instance.DPV(_database);

                    differentialPulse.Ranging.StartCurrentRange = new CurrentRange(3);
                    differentialPulse.Ranging.MaximumCurrentRange = new CurrentRange(3);
                    differentialPulse.Ranging.MaximumCurrentRange = new CurrentRange(1);

                    return differentialPulse;

                case "Linear Voltammetry":
                    LinearSweep linSweep = instance.LinSweep(_database);

                    linSweep.Ranging.StartCurrentRange = new CurrentRange(5);
                    linSweep.Ranging.MaximumCurrentRange = new CurrentRange(6);
                    linSweep.Ranging.MaximumCurrentRange = new CurrentRange(3);

                    return null;

                case "Square Wave Voltammetry":
                    SquareWave squareWave = instance.SWV(_database);

                    squareWave.Ranging.StartCurrentRange = new CurrentRange(5);
                    squareWave.Ranging.MaximumCurrentRange = new CurrentRange(6);
                    squareWave.Ranging.MaximumCurrentRange = new CurrentRange(3);

                    return null;

                default:
                    //Add code to notify user that something has gone wrong and needs to be fixed

                    return null;
            }
        }

        public async void SimpleConnect(int fileNum, bool RunningPC, bool RunningNC, bool RunningReal, bool RunningDPV)
        {
            bool RunningBL = true;
            string testRun = "3";
            //Below sets which option the code will execute
            SimpleMeasurement baseline;
            AssetManager assetManager = Application.Context.Assets;
            using (StreamReader sr = new StreamReader(assetManager.Open(testRun + "_2525AfterMch" + fileNum + ".pssession")))
                baseline = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

            baselineCurves = baseline.SimpleCurveCollection;

            if (RunningReal)
            {
                Context context = Application.Context;
                IAttributeSet attributeSet = null;
                PSCommSimpleAndroid psCommSimpleAndroid = new PSCommSimpleAndroid(context, attributeSet);
                Device[] devices = await psCommSimpleAndroid.GetConnectedDevices();

                try
                {
                    psCommSimpleAndroid.Connect(devices[0]);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                psCommSimpleAndroid.MeasurementStarted += PsCommSimpleAndroid_MeasurementStarted;
                psCommSimpleAndroid.MeasurementEnded += PsCommSimpleAndroid_MeasurementEnded;
                psCommSimpleAndroid.SimpleCurveStartReceivingData += PsCommSimpleAndroid_SimpleCurveStartReceivingData;
                var runScan = await RunScan();

                activeSimpleMeasurement = psCommSimpleAndroid.Measure(runScan);
            }
            else if (RunningDPV)
            {
                using (StreamReader sr = new StreamReader(assetManager.Open("blank.pssession")))
                    baseline = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];
                Context context = Application.Context;
                IAttributeSet attributeSet = null;
                PSCommSimpleAndroid psCommSimpleAndroid = new PSCommSimpleAndroid(context, attributeSet);
                Device[] devices = await psCommSimpleAndroid.GetConnectedDevices();

                try
                {
                    psCommSimpleAndroid.Connect(devices[0]);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                psCommSimpleAndroid.MeasurementStarted += PsCommSimpleAndroid_MeasurementStarted;
                psCommSimpleAndroid.MeasurementEnded += PsCommSimpleAndroid_MeasurementEnded;
                psCommSimpleAndroid.SimpleCurveStartReceivingData += PsCommSimpleAndroid_SimpleCurveStartReceivingData;
                DifferentialPulse runScan = await RunScan();

                activeSimpleMeasurement = psCommSimpleAndroid.Measure(runScan);

                //Add in processing stuff here
                SimpleLoadSaveFunctions.SaveMeasurement(activeSimpleMeasurement, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

                List<SimpleCurve> simpleCurves = activeSimpleMeasurement.SimpleCurveCollection;

                SimpleCurve subtractedCurve = simpleCurves[0].Subtract(baselineCurves[0]);    //Note, replace simpleCurves[1] w/ the standard blank curve

                SimpleCurve baselineCurve = subtractedCurve.MovingAverageBaseline();

                baselineCurve.DetectPeaks(0.05, 0, true, false);

                PeakList peakList = baselineCurve.Peaks;
                Peak mainPeak = peakList[peakList.nPeaks - 1];   //Note, the proper peak is the last peak, not the first peak
                double peakLocation = mainPeak.PeakX;
                double peakHeight = mainPeak.PeakValue;

                var allDb = await App.Database.GetScanDatabasesAsync();
                var _database = await App.Database.GetScanAsync(allDb.Count);

                if (peakLocation <= -0.3 && peakLocation >= -0.4)
                {
                    _database.IsInfected = true;
                }
                else
                {
                    _database.IsInfected = false;
                }

                _database.PeakVoltage = peakHeight;
                await App.Database.SaveScanAsync(_database);
            }
            else if (RunningNC || RunningPC || RunningBL)
            {
                if (RunningBL)
                {
                    SimpleMeasurement baselineMeasurement;
                    using (StreamReader sr = new StreamReader(assetManager.Open(testRun + "_baselineOnlySmooth" + fileNum + ".pssession")))
                        baselineMeasurement = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

                    List<SimpleCurve> avgBaselineCurves = baselineMeasurement.SimpleCurveCollection;

                    SimpleCurve avgBaselineCurve = avgBaselineCurves[0];

                    avgBaselineCurve.DetectPeaks(0.05, 0, true, false);

                    PeakList avgBaselinePeakList = avgBaselineCurve.Peaks;

                    if (avgBaselinePeakList.nPeaks != 0)
                    {
                        Peak avgBaselinePeak = avgBaselinePeakList[avgBaselinePeakList.nPeaks - 1];

                        double avgBaselinePeakLocation = avgBaselinePeak.PeakX;
                        double avgBaselinePeakValue = avgBaselinePeak.PeakValue;

                        var allDb = await App.Database.GetScanDatabasesAsync();
                        var _database = await App.Database.GetScanAsync(allDb.Count);

                        if (avgBaselinePeakLocation <= -0.3 && avgBaselinePeakLocation >= -0.4)
                        {
                            _database.IsInfected = true;
                        }
                        else
                        {
                            _database.IsInfected = false;
                        }

                        _database.PeakVoltage = avgBaselinePeakValue;
                        await App.Database.SaveScanAsync(_database);
                    }
                    else
                    {
                        var allDb = await App.Database.GetScanDatabasesAsync();
                        var _database = await App.Database.GetScanAsync(allDb.Count);

                        _database.IsInfected = false;
                        _database.PeakVoltage = 0.0;
                        _database.VoltamType = "Failed PC";
                        await App.Database.SaveScanAsync(_database);
                    }
                }
                else if (RunningPC)
                {
                    SimpleMeasurement positiveControl;
                    using (StreamReader sr = new StreamReader(assetManager.Open(testRun + "_2525AfterTarget" + fileNum + ".pssession")))
                        positiveControl = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

                    List<SimpleCurve> positiveCurves = positiveControl.SimpleCurveCollection;

                    SimpleCurve subtractedCurve = positiveCurves[0].Subtract(baselineCurves[0]);

                    SimpleCurve baselineCurve = subtractedCurve.MovingAverageBaseline();

                    baselineCurve.DetectPeaks(0.05, 0, true, false);

                    PeakList positivePeakList = baselineCurve.Peaks;

                    if (positivePeakList.nPeaks != 0)
                    {
                        Peak positivePeak = positivePeakList[positivePeakList.nPeaks - 1];

                        double positivePeakLocation = positivePeak.PeakX;
                        double positivePeakValue = positivePeak.PeakValue;

                        var allDb = await App.Database.GetScanDatabasesAsync();
                        var _database = await App.Database.GetScanAsync(allDb.Count);

                        if (positivePeakLocation <= -0.3 && positivePeakLocation >= -0.4)
                        {
                            _database.IsInfected = true;
                        }
                        else
                        {
                            _database.IsInfected = false;
                        }

                        _database.PeakVoltage = positivePeakValue;
                        await App.Database.SaveScanAsync(_database);
                    }
                    else
                    {
                        var allDb = await App.Database.GetScanDatabasesAsync();
                        var _database = await App.Database.GetScanAsync(allDb.Count);

                        _database.IsInfected = false;
                        _database.PeakVoltage = 0.0;
                        _database.VoltamType = "Failed PC";
                        await App.Database.SaveScanAsync(_database);
                    }
                }
                else if (RunningNC)
                {
                    SimpleMeasurement negativeControl;
                    using (StreamReader sr = new StreamReader(assetManager.Open(testRun + "_2525AfterMch" + fileNum + ".pssession")))
                        negativeControl = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

                    List<SimpleCurve> negativeCurves = negativeControl.SimpleCurveCollection;

                    SimpleCurve subtractedCurve = negativeCurves[0].Subtract(baselineCurves[0]);

                    SimpleCurve baselineCurve = subtractedCurve.MovingAverageBaseline();

                    baselineCurve.DetectPeaks(0.05, 0, true, false);

                    PeakList negativePeakList = baselineCurve.Peaks;
                    if (negativePeakList.nPeaks != 0)
                    {
                        Peak negativePeak = negativePeakList[negativePeakList.nPeaks - 1];
                        double negativePeakLocation = negativePeak.PeakX;
                        double negativePeakValue = negativePeak.PeakValue;

                        var allDb = await App.Database.GetScanDatabasesAsync();
                        var _database = await App.Database.GetScanAsync(allDb.Count);

                        if (negativePeakLocation <= -0.3 && negativePeakLocation >= -0.4)
                        {
                            _database.IsInfected = true;
                        }
                        else
                        {
                            _database.IsInfected = false;
                        }

                        _database.PeakVoltage = negativePeakValue;
                        await App.Database.SaveScanAsync(_database);
                    }
                    else
                    {
                        var allDb = await App.Database.GetScanDatabasesAsync();
                        var _database = await App.Database.GetScanAsync(allDb.Count);

                        _database.IsInfected = false;
                        _database.PeakVoltage = 0.0;
                        _database.VoltamType = "Failed NC";
                        await App.Database.SaveScanAsync(_database);
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