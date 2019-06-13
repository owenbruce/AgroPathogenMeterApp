using Android.Content;
using PalmSens;
using PalmSens.Comm;
using PalmSens.Devices;
using PalmSens.Techniques;
using PalmSens.PSAndroid.Comm;
using PalmSens.Plottables;
using System;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;
using AgroPathogenMeterApp.Droid;
using AgroPathogenMeterApp.Models;
using Xamarin.Forms;

[assembly: Dependency (typeof (BtConnect))]
namespace AgroPathogenMeterApp.Droid
{
    public class BtConnect: BtControl
    {
        Context context = Android.App.Application.Context;
        Measurement measurement;
        Curve _activeCurve;

        public async void connect()
        {
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
            
        }

        public async void connect(ScanDatabase _database)
        {
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
                comm.ReceiveStatus += Comm_ReceiveStatus;

                comm.BeginMeasurement += Comm_BeginMeasurement;
                //Run all of the setup settings here from _database

                comm.BeginReceiveCurve += Comm_BeginReceiveCurve;
                //Collect all of the information here

                #region Test Switches
                switch (_database.VoltamType)
                {
                    case "Square Wave Voltammetry":
                        SquareWave squareWave = new SquareWave();

                        squareWave.BeginPotential = (float)_database.StartingPotential;
                        squareWave.EndPotential = (float)_database.EndingPotential;
                        squareWave.StepPotential = (float)_database.PotentialStep;
                        squareWave.PulseAmplitude = (float)_database.Amplitude;
                        squareWave.Frequency = (float)_database.Frequency;

                        comm.Measure(squareWave);

                        break;

                    case "Linear Voltammetry":
                        LinearSweep linearSweep = new LinearSweep();

                        linearSweep.BeginPotential = (float)_database.StartingPotential;
                        linearSweep.EndPotential = (float)_database.EndingPotential;
                        linearSweep.StepPotential = (float)_database.PotentialStep;
                        linearSweep.Scanrate = (float)_database.ScanRate;

                        comm.Measure(linearSweep);

                        break;

                    case "Cyclic Voltammetry":
                        CyclicVoltammetry cyclicVoltammetry = new CyclicVoltammetry();

                        cyclicVoltammetry.BeginPotential = (float)_database.StartingPotential;
                        cyclicVoltammetry.Vtx1Potential = (float)_database.NegativeVertex;
                        cyclicVoltammetry.Vtx2Potential = (float)_database.PositiveVertex;
                        cyclicVoltammetry.StepPotential = (float)_database.PotentialStep;
                        cyclicVoltammetry.Scanrate = (float)_database.ScanRate;
                        cyclicVoltammetry.nScans = 1;

                        comm.Measure(cyclicVoltammetry);

                        break;

                    case "Alternating Current Voltammetry":
                        ACVoltammetry acVoltammetry = new ACVoltammetry();

                        acVoltammetry.BeginPotential = (float)_database.StartingPotential;
                        acVoltammetry.EndPotential = (float)_database.EndingPotential;
                        acVoltammetry.StepPotential = (float)_database.PotentialStep;
                        acVoltammetry.SineWaveAmplitude = (float)_database.ACPotential;
                        acVoltammetry.Scanrate = (float)_database.ScanRate;
                        acVoltammetry.Frequency = (float)_database.Frequency;

                        comm.Measure(acVoltammetry);

                        break;

                    case "Chronoamperometry":

                        

                        break;
                }

                #endregion
                comm.Disconnect();
            }
            catch (Exception ex)
            {
                device.Close();
                Crashes.TrackError(ex);
            }

        }

        private void Comm_BeginReceiveCurve(object sender, CurveEventArgs e)
        {
            _activeCurve = e.GetCurve();
            _activeCurve.NewDataAdded += _activeCurve_NewDataAdded;
            _activeCurve.Finished += _activeCurve_Finished;

        }

        private void _activeCurve_Finished(object sender, Plottable.FinishedEventArgs e)
        {
            _activeCurve.NewDataAdded -= _activeCurve_NewDataAdded;
            _activeCurve.Finished -= _activeCurve_Finished;
        }

        private void _activeCurve_NewDataAdded(object sender, PalmSens.Data.ArrayDataAddedEventArgs e)
        {
            int startIndex = e.StartIndex;
            int count = e.Count;
            double[] newData = new double[count];
            (sender as Curve).GetYValues().CopyTo(newData, startIndex);
        }

        private void Comm_BeginMeasurement(object sender, ActiveMeasurement newMeasurement)
        {
            measurement = newMeasurement;
        }

        private void Comm_ReceiveStatus(Object sender, StatusEventArgs e)
        {
            Status status = e.GetStatus();
        }
    }
}