﻿using AgroPathogenMeterApp.Droid;
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
using System.Threading;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(BtControl_Android))]

namespace AgroPathogenMeterApp.Droid
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class BtControl_Android : IBtControl
    {
        //Initializes the parameters required for the scan and processing
        private Curve _activeCurve;

        private SimpleCurve _activeSimpleCurve;
        private SimpleMeasurement activeSimpleMeasurement;
        private List<SimpleCurve> baselineCurves;

        #region Flags

        //Notifiex when the curve has finished having new data added
        protected virtual void _activeCurve_Finished(object sender, EventArgs e)
        {
            _activeCurve.NewDataAdded -= _activeCurve_NewDataAdded;
            _activeCurve.Finished -= _activeCurve_Finished;
        }

        //Notifies when a curve has new data added
        protected virtual void _activeCurve_NewDataAdded(object sender, ArrayDataAddedEventArgs e)
        {
            int startIndex = e.StartIndex;
            int count = e.Count;
            double[] newData = new double[count];
            (sender as Curve).GetYValues().CopyTo(newData, startIndex);
        }

        //Notified when the simple curve has finished having new data added
        protected virtual void _activeSimpleCurve_CurveFinished(object sender, EventArgs e)
        {
            _activeSimpleCurve.NewDataAdded -= _activeCurve_NewDataAdded;
            _activeSimpleCurve.CurveFinished -= _activeSimpleCurve_CurveFinished;
        }

        //Notifies when a simple curve has new data added
        protected virtual void _activeSimpleCurve_NewDataAdded(object sender, ArrayDataAddedEventArgs e)
        {
            int startIndex = e.StartIndex;
            int count = e.Count;
            double[] newData = new double[count];
            (sender as SimpleCurve).YAxisValues.CopyTo(newData, startIndex);
        }

        //Notifies when a measurement begins on the potentiostat
        protected virtual void Comm_BeginMeasurement(object sender, ActiveMeasurement newMeasurement)
        {
        }

        //Notifies when a curve begins to receive data from the potentiostat
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

        //Notifies when a measurement is finished, nothing currently implemented
        protected virtual void PsCommSimpleAndroid_MeasurementEnded(object sender, EventArgs e)
        {
        }

        //Notifies when a measurement is started, nothing currently implemented
        protected virtual void PsCommSimpleAndroid_MeasurementStarted(object sender, EventArgs e)
        {
        }

        //Notifies when a curve starts to receive data
        protected virtual void PsCommSimpleAndroid_SimpleCurveStartReceivingData(object sender, SimpleCurve activeSimpleCurve)
        {
            _activeSimpleCurve = activeSimpleCurve;
            _activeSimpleCurve.NewDataAdded += _activeSimpleCurve_NewDataAdded;
            _activeSimpleCurve.CurveFinished += _activeSimpleCurve_CurveFinished;
        }

        #endregion Flags

        //Saves the name and the macaddress of the connected potentiostat
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

        //Normal connection to the potentiostat, move to simple connect only if normal connect isn't necessary
        public void Connect(int fileNum, bool RunningPC, bool RunningNC, bool RunningReal, bool RunningDPV)
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

        //Not needed currently, remove if obsolete
        public string FilePath()
        {
            return "";
        }

        //Sets the scan parameters
        public async Task<DifferentialPulse> RunScan()
        {
            //Grabs the most recent database to get the required parameters
            var allDb = await App.Database.GetScanDatabasesAsync();
            var _database = await App.Database.GetScanAsync(allDb.Count);

            //Gets an instance of ScanParams
            var instance = new ScanParams();

            switch (_database.VoltamType)
            {
                case "Alternating Current Voltammetry":   //Sets an alternating current voltammetric scan
                    using (ACVoltammetry acVoltammetry = instance.ACV(_database))
                    {
                        acVoltammetry.Ranging.StartCurrentRange = new CurrentRange(5);
                        acVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(6);
                        acVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(3);

                        return null;
                    }

                case "Cyclic Voltammetry":   //Sets a cyclic voltammetric scan
                    using (CyclicVoltammetry cVoltammetry = instance.CV(_database))
                    {
                        cVoltammetry.Ranging.StartCurrentRange = new CurrentRange(5);
                        cVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(6);
                        cVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(3);

                        return null;
                    }

                case "Differential Pulse Voltammetry":   //Sets a differential pulse voltammetric scan
                    using (DifferentialPulse differentialPulse = instance.DPV(_database))
                    {
                        differentialPulse.Ranging.StartCurrentRange = new CurrentRange(3);
                        differentialPulse.Ranging.MaximumCurrentRange = new CurrentRange(3);
                        differentialPulse.Ranging.MaximumCurrentRange = new CurrentRange(1);

                        return differentialPulse;
                    }

                case "Linear Voltammetry":   //Sets a linear voltammetric scan
                    using (LinearSweep linSweep = instance.LinSweep(_database))
                    {
                        linSweep.Ranging.StartCurrentRange = new CurrentRange(5);
                        linSweep.Ranging.MaximumCurrentRange = new CurrentRange(6);
                        linSweep.Ranging.MaximumCurrentRange = new CurrentRange(3);

                        return null;
                    }

                case "Square Wave Voltammetry":   //Sets a square wave voltammetric scan
                    using (SquareWave squareWave = instance.SWV(_database))
                    {
                        squareWave.Ranging.StartCurrentRange = new CurrentRange(5);
                        squareWave.Ranging.MaximumCurrentRange = new CurrentRange(6);
                        squareWave.Ranging.MaximumCurrentRange = new CurrentRange(3);

                        return null;
                    }

                default:
                    //Add code to notify user that something has gone wrong and needs to be fixed

                    return null;
            }
        }

        //Simple connection to the palmsens, currently the only one used
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

            //Runs a real scan depending on whatever parameters the person has set
            if (RunningReal)
            {
                Context context = Application.Context;
                IAttributeSet attributeSet = null;
                PSCommSimpleAndroid psCommSimpleAndroid = new PSCommSimpleAndroid(context, attributeSet);   //Uses a simple comm with the palmsens
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
                using (var runScan = await RunScan())
                {
                    activeSimpleMeasurement = psCommSimpleAndroid.Measure(runScan);
                }

                Thread.Sleep(20000);

                psCommSimpleAndroid.Dispose();
            }

            //Runs a differential pulse voltammetric scan for testing
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
                using (DifferentialPulse runScan = await RunScan())
                {
                    activeSimpleMeasurement = psCommSimpleAndroid.Measure(runScan);
                }

                Thread.Sleep(50000);   //Temporary workaround

                psCommSimpleAndroid.Dispose();

                //Add in processing stuff here
                SimpleLoadSaveFunctions.SaveMeasurement(activeSimpleMeasurement, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

                List<SimpleCurve> simpleCurves = activeSimpleMeasurement.SimpleCurveCollection;

                SimpleCurve baselineCurve;

                SimpleCurve subtractedCurve = simpleCurves[0].Subtract(baselineCurves[0]);    //Note, replace simpleCurves[1] w/ the standard blank curve

                baselineCurve = subtractedCurve.MovingAverageBaseline();

                subtractedCurve.Dispose();

                PeakList peakList = baselineCurve.Peaks;

                baselineCurve.Dispose();

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

                    SimpleCurve baselineCurve;

                    SimpleCurve subtractedCurve = positiveCurves[0].Subtract(baselineCurves[0]);

                    baselineCurve = subtractedCurve.MovingAverageBaseline();

                    subtractedCurve.Dispose();

                    baselineCurve.DetectPeaks(0.05, 0, true, false);

                    PeakList positivePeakList = baselineCurve.Peaks;

                    baselineCurve.Dispose();

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

                    SimpleCurve baselineCurve;

                    SimpleCurve subtractedCurve = negativeCurves[0].Subtract(baselineCurves[0]);

                    baselineCurve = subtractedCurve.MovingAverageBaseline();

                    subtractedCurve.Dispose();

                    baselineCurve.DetectPeaks(0.05, 0, true, false);

                    PeakList negativePeakList = baselineCurve.Peaks;

                    baselineCurve.Dispose();

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

        //Test that a connection to the palmsens exists by getting the name and macaddress
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