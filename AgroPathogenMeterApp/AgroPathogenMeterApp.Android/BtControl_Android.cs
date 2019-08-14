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
using System.Threading;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(BtControl_Android))]

namespace AgroPathogenMeterApp.Droid
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class BtControl_Android : IBtControl
    {
        public static readonly TimeSpan MaxWait = TimeSpan.FromSeconds(15);
        private AutoResetEvent _measurementEnded;

        //Initializes the parameters required for the scan and processing
        private Curve _activeCurve;

        private SimpleCurve _activeSimpleCurve;
        private SimpleMeasurement activeSimpleMeasurement;
        private List<SimpleCurve> baselineCurves;
        private bool running;

        public BtControl_Android()
        {
            Context context = Application.Context;   //Loads the current android context
            IAttributeSet attributeSet = null;
            PSCommSimpleAndroid psCommSimpleAndroid = new PSCommSimpleAndroid(context, attributeSet);   //Uses a simple comm with the palmsens

            this._measurementEnded = new AutoResetEvent(false);
            psCommSimpleAndroid.MeasurementEnded += this.PsCommSimpleAndroid_MeasurementEnded;
        }

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
        protected virtual async void PsCommSimpleAndroid_MeasurementEnded(object sender, EventArgs e)
        {
            FileHack instance = new FileHack();

            activeSimpleMeasurement = instance.HackSWV(activeSimpleMeasurement);

            List<SimpleCurve> simpleCurves = activeSimpleMeasurement.SimpleCurveCollection;

            SimpleCurve subtractedCurve = simpleCurves[0].Subtract(baselineCurves[0]);    //Note, replace simpleCurves[1] w/ the standard blank curve

            SimpleCurve baselineCurve = subtractedCurve.MovingAverageBaseline();   //Subtracts the baseline from the subtracted curve

            subtractedCurve.Dispose();   //Disposes of the subtracted curve

            PeakList peakList = baselineCurve.Peaks;   //Detects the peaks on the subtracted curve

            baselineCurve.Dispose();   //Disposes of the baseline curve

            Peak mainPeak = peakList[peakList.nPeaks - 1];   //Note, the proper peak is the last peak, not the first peak
            double peakLocation = mainPeak.PeakX;
            double peakHeight = mainPeak.PeakValue;

            List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
            ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);

            if (peakLocation <= -0.3 && peakLocation >= -0.4)   //If the peak is between a certain range, the sample is infected, add in a minimum value once one is determined
            {
                _database.IsInfected = true;
            }
            else
            {
                _database.IsInfected = false;
            }

            _database.PeakVoltage = peakHeight;
            await App.Database.SaveScanAsync(_database);   //Saves the current database
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
        public async Task<Method> RunScan()
        {
            //Grabs the most recent database to get the required parameters
            List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
            ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);

            //Gets an instance of ScanParams
            ScanParams instance = new ScanParams();

            switch (_database.VoltamType)   //Switch which invokes the correct type of scan
            {
                case "Alternating Current Voltammetry":   //Sets an alternating current voltammetric scan
                    using (ACVoltammetry acVoltammetry = instance.ACV(_database))
                    {
                        acVoltammetry.Ranging.StartCurrentRange = new CurrentRange(5);   //Sets the range that the potentiostat will use to detect the current
                        acVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(6);
                        acVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(3);

                        return acVoltammetry;
                    }

                case "Cyclic Voltammetry":   //Sets a cyclic voltammetric scan
                    using (CyclicVoltammetry cVoltammetry = instance.CV(_database))
                    {
                        cVoltammetry.Ranging.StartCurrentRange = new CurrentRange(5);   //Sets the range that the potentiostat will use to detect the current
                        cVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(6);
                        cVoltammetry.Ranging.MaximumCurrentRange = new CurrentRange(3);

                        return cVoltammetry;
                    }

                case "Differential Pulse Voltammetry":   //Sets a differential pulse voltammetric scan
                    using (DifferentialPulse differentialPulse = instance.DPV(_database))
                    {
                        differentialPulse.Ranging.StartCurrentRange = new CurrentRange(3);   //Sets the range that the potentiostat will use to detect the current
                        differentialPulse.Ranging.MaximumCurrentRange = new CurrentRange(3);
                        differentialPulse.Ranging.MaximumCurrentRange = new CurrentRange(1);

                        return differentialPulse;
                    }

                case "Linear Voltammetry":   //Sets a linear voltammetric scan
                    using (LinearSweep linSweep = instance.LinSweep(_database))
                    {
                        linSweep.Ranging.StartCurrentRange = new CurrentRange(5);   //Sets the range that the potentiostat will use to detect the current
                        linSweep.Ranging.MaximumCurrentRange = new CurrentRange(6);
                        linSweep.Ranging.MaximumCurrentRange = new CurrentRange(3);

                        return linSweep;
                    }

                case "Square Wave Voltammetry":   //Sets a square wave voltammetric scan
                    using (SquareWave squareWave = instance.SWV(_database))
                    {
                        squareWave.Ranging.StartCurrentRange = new CurrentRange(5);   //Sets the range that the potentiostat will use to detect the current
                        squareWave.Ranging.MaximumCurrentRange = new CurrentRange(6);
                        squareWave.Ranging.MaximumCurrentRange = new CurrentRange(3);

                        return squareWave;
                    }

                default:
                    //Add code to notify user that something has gone wrong and needs to be fixed

                    return null;
            }
        }

        private void CheckRunning()
        {
        }

        //Simple connection to the palmsens, currently the only one used
        public async void SimpleConnect(int fileNum, bool RunningPC, bool RunningNC, bool RunningReal, bool RunningDPV)
        {
            bool RunningBL = true;
            string testRun = "3";
            //Below sets which option the code will execute
            SimpleMeasurement baseline;
            AssetManager assetManager = Application.Context.Assets;
            using (StreamReader sr = new StreamReader(assetManager.Open(testRun + "_2525AfterMch" + fileNum + ".pssession")))   //Loads a blank baseline curve to subtract from the scanned/loaded curve
                baseline = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

            baselineCurves = baseline.SimpleCurveCollection;

            //Runs a real scan depending on whatever parameters the person has set
            if (RunningReal)
            {
                Context context = Application.Context;   //Loads the current android context
                IAttributeSet attributeSet = null;
                PSCommSimpleAndroid psCommSimpleAndroid = new PSCommSimpleAndroid(context, attributeSet);   //Uses a simple comm with the palmsens
                Device[] devices = await psCommSimpleAndroid.GetConnectedDevices();

                try
                {
                    psCommSimpleAndroid.Connect(devices[0]);   //Connect to the first palmsens found
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                psCommSimpleAndroid.MeasurementStarted += PsCommSimpleAndroid_MeasurementStarted;   //Loads the necessary flags
                psCommSimpleAndroid.MeasurementEnded += PsCommSimpleAndroid_MeasurementEnded;
                psCommSimpleAndroid.SimpleCurveStartReceivingData += PsCommSimpleAndroid_SimpleCurveStartReceivingData;

                Method runScan = await RunScan();   //Sets the scan parameters

                activeSimpleMeasurement = psCommSimpleAndroid.Measure(runScan);   //Runs the scan on the potentiostat

                //Thread.Sleep(10100);   //Pauses while the scan is running, temporary measure

                this._measurementEnded.WaitOne(MaxWait);
            }

            //Runs a differential pulse voltammetric scan for testing
            else if (RunningDPV)
            {
                using (StreamReader sr = new StreamReader(assetManager.Open("blank.pssession")))   //Loads a blank curve as a baseline to be subtracted
                    baseline = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

                Context context = Application.Context;   //Loads the current android context
                IAttributeSet attributeSet = null;
                PSCommSimpleAndroid psCommSimpleAndroid = new PSCommSimpleAndroid(context, attributeSet);   //Initializes the palmsens comm
                Device[] devices = await psCommSimpleAndroid.GetConnectedDevices();

                try
                {
                    psCommSimpleAndroid.Connect(devices[0]);   //Connects to the first palmsens found
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }

                psCommSimpleAndroid.MeasurementStarted += PsCommSimpleAndroid_MeasurementStarted;   //Loads the necessary flags
                psCommSimpleAndroid.MeasurementEnded += PsCommSimpleAndroid_MeasurementEnded;
                psCommSimpleAndroid.SimpleCurveStartReceivingData += PsCommSimpleAndroid_SimpleCurveStartReceivingData;

                Method runScan = await RunScan();   //Sets the scan parameters

                activeSimpleMeasurement = psCommSimpleAndroid.Measure(runScan);   //Runs the scan on the potentiostat

                running = true;

                Thread.Sleep(50000);   //Temporary workaround

                while (running == true)
                {
                    CheckRunning();
                }

                psCommSimpleAndroid.Dispose();   //Disposes of the comm when it is done being used

                List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();   //Loads the current database
                //Add in processing stuff here
                //SimpleLoadSaveFunctions.SaveMeasurement(activeSimpleMeasurement, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "dpv" + allDb.Count + ".pssession"));

                FileHack instance = new FileHack();

                activeSimpleMeasurement = instance.HackDPV(activeSimpleMeasurement);

                List<SimpleCurve> simpleCurves = activeSimpleMeasurement.SimpleCurveCollection;

                SimpleCurve subtractedCurve = simpleCurves[0].Subtract(baselineCurves[0]);    //Note, replace simpleCurves[1] w/ the standard blank curve

                SimpleCurve baselineCurve = subtractedCurve.MovingAverageBaseline();   //Subtracts the baseline from the subtracted curve

                subtractedCurve.Dispose();   //Disposes of the subtracted curve

                PeakList peakList = baselineCurve.Peaks;   //Detects the peaks on the subtracted curve

                baselineCurve.Dispose();   //Disposes of the baseline curve

                Peak mainPeak = peakList[peakList.nPeaks - 1];   //Note, the proper peak is the last peak, not the first peak
                double peakLocation = mainPeak.PeakX;
                double peakHeight = mainPeak.PeakValue;

                ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);

                if (peakLocation <= -0.3 && peakLocation >= -0.4)   //If the peak is between a certain range, the sample is infected, add in a minimum value once one is determined
                {
                    _database.IsInfected = true;
                }
                else
                {
                    _database.IsInfected = false;
                }

                _database.PeakVoltage = peakHeight;
                await App.Database.SaveScanAsync(_database);   //Saves the current database
            }
            else if (RunningNC || RunningPC || RunningBL)   //If a test scan is being run
            {
                if (RunningBL)   //If a simple baseline test is run with no subtraction or curve manipulation
                {
                    SimpleMeasurement baselineMeasurement;
                    using (StreamReader sr = new StreamReader(assetManager.Open(testRun + "_baselineOnlySmooth" + fileNum + ".pssession")))
                        baselineMeasurement = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

                    List<SimpleCurve> avgBaselineCurves = baselineMeasurement.SimpleCurveCollection;

                    SimpleCurve avgBaselineCurve = avgBaselineCurves[0];

                    avgBaselineCurve.DetectPeaks(0.05, 0, true, false);   //Detect peaks only if they are wider than 0.05 V

                    PeakList avgBaselinePeakList = avgBaselineCurve.Peaks;

                    if (avgBaselinePeakList.nPeaks != 0)   //If it detects a peak run below code
                    {
                        Peak avgBaselinePeak = avgBaselinePeakList[avgBaselinePeakList.nPeaks - 1];   //Use the last peak detected which should be the wanted peak

                        double avgBaselinePeakLocation = avgBaselinePeak.PeakX;
                        double avgBaselinePeakValue = avgBaselinePeak.PeakValue;

                        List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
                        ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);

                        if (avgBaselinePeakLocation <= -0.3 && avgBaselinePeakLocation >= -0.4)   //If a peak is between a certain range, the sample is infected, add in a minimal value once determined
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
                    else   //If no peak is detected
                    {
                        List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
                        ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);

                        _database.IsInfected = false;
                        _database.PeakVoltage = 0.0;
                        _database.VoltamType = "Failed PC";
                        await App.Database.SaveScanAsync(_database);
                    }
                }
                else if (RunningPC)   //If a postitive control test is being run with known values
                {
                    SimpleMeasurement positiveControl;
                    using (StreamReader sr = new StreamReader(assetManager.Open(testRun + "_2525AfterTarget" + fileNum + ".pssession")))
                        positiveControl = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

                    List<SimpleCurve> positiveCurves = positiveControl.SimpleCurveCollection;

                    SimpleCurve baselineCurve;

                    SimpleCurve subtractedCurve = positiveCurves[0].Subtract(baselineCurves[0]);

                    baselineCurve = subtractedCurve.MovingAverageBaseline();

                    subtractedCurve.Dispose();

                    baselineCurve.DetectPeaks(0.05, 0, true, false);   //Detect all peaks with width greater than 0.05 V

                    PeakList positivePeakList = baselineCurve.Peaks;

                    baselineCurve.Dispose();

                    if (positivePeakList.nPeaks != 0)   //If a peak is detected, execute the code below
                    {
                        Peak positivePeak = positivePeakList[positivePeakList.nPeaks - 1];

                        double positivePeakLocation = positivePeak.PeakX;
                        double positivePeakValue = positivePeak.PeakValue;

                        List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
                        ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);

                        if (positivePeakLocation <= -0.3 && positivePeakLocation >= -0.4)   //If a peak is between a certain range, the sample is infected, add in minimum value once determined
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
                    else   //If no peak is detected, execute the code below
                    {
                        List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
                        ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);

                        _database.IsInfected = false;
                        _database.PeakVoltage = 0.0;
                        _database.VoltamType = "Failed PC";
                        await App.Database.SaveScanAsync(_database);
                    }
                }
                else if (RunningNC)   //If a negative control test is being run
                {
                    SimpleMeasurement negativeControl;
                    using (StreamReader sr = new StreamReader(assetManager.Open(testRun + "_2525AfterMch" + fileNum + ".pssession")))
                        negativeControl = SimpleLoadSaveFunctions.LoadMeasurements(sr)[0];

                    List<SimpleCurve> negativeCurves = negativeControl.SimpleCurveCollection;

                    SimpleCurve baselineCurve;

                    SimpleCurve subtractedCurve = negativeCurves[0].Subtract(baselineCurves[0]);

                    baselineCurve = subtractedCurve.MovingAverageBaseline();

                    subtractedCurve.Dispose();

                    baselineCurve.DetectPeaks(0.05, 0, true, false);   //Detect all peaks with a width greater than 0.05 V

                    PeakList negativePeakList = baselineCurve.Peaks;

                    baselineCurve.Dispose();

                    if (negativePeakList.nPeaks != 0)   //If a peak is detected, execute the code below
                    {
                        Peak negativePeak = negativePeakList[negativePeakList.nPeaks - 1];
                        double negativePeakLocation = negativePeak.PeakX;
                        double negativePeakValue = negativePeak.PeakValue;

                        List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
                        ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);

                        if (negativePeakLocation <= -0.3 && negativePeakLocation >= -0.4)   //If a peak is in a certain range, the sample is infected, add a minimum value once determined
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
                    else   //If a peak is not detected
                    {
                        List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
                        ScanDatabase _database = await App.Database.GetScanAsync(allDb.Count);

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
                    foreach (BluetoothDevice pairedDevice in BluetoothAdapter.DefaultAdapter.BondedDevices)
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