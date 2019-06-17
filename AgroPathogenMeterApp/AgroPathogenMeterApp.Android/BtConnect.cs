using AgroPathogenMeterApp.Droid;
using AgroPathogenMeterApp.Models;
using Android.Content;
using Android.OS;
using Microsoft.AppCenter.Crashes;
using PalmSens;
using PalmSens.Comm;
using PalmSens.Plottables;
using PalmSens.PSAndroid.Comm;
using PalmSens.Techniques;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(BtConnect))]

namespace AgroPathogenMeterApp.Droid
{
    public class BtConnect : BtControl
    {
        private Curve _activeCurve;
        private Context context = Android.App.Application.Context;
        private Measurement measurement;

        public async void connect()
        {
            if (IsEmulator())
            {
            }
            else
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
        }

        public async void connect(ScanDatabase _database)
        {
            if (IsEmulator())
            {
            }
            else
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
                            squareWave.Frequency = (float)_database.Frequency;
                            squareWave.PulseAmplitude = (float)_database.Amplitude;
                            squareWave.StepPotential = (float)_database.PotentialStep;

                            comm.Measure(squareWave);

                            break;

                        case "Linear Voltammetry":
                            LinearSweep linearSweep = new LinearSweep();

                            linearSweep.BeginPotential = (float)_database.StartingPotential;
                            linearSweep.EndPotential = (float)_database.EndingPotential;
                            linearSweep.Scanrate = (float)_database.ScanRate;
                            linearSweep.StepPotential = (float)_database.PotentialStep;

                            comm.Measure(linearSweep);

                            break;

                        case "Cyclic Voltammetry":
                            CyclicVoltammetry cyclicVoltammetry = new CyclicVoltammetry();

                            cyclicVoltammetry.BeginPotential = (float)_database.StartingPotential;
                            cyclicVoltammetry.nScans = 1;
                            cyclicVoltammetry.Scanrate = (float)_database.ScanRate;
                            cyclicVoltammetry.StepPotential = (float)_database.PotentialStep;
                            cyclicVoltammetry.Vtx1Potential = (float)_database.NegativeVertex;
                            cyclicVoltammetry.Vtx2Potential = (float)_database.PositiveVertex;

                            comm.Measure(cyclicVoltammetry);

                            break;

                        case "Alternating Current Voltammetry":
                            ACVoltammetry acVoltammetry = new ACVoltammetry();

                            acVoltammetry.BeginPotential = (float)_database.StartingPotential;
                            acVoltammetry.EndPotential = (float)_database.EndingPotential;
                            acVoltammetry.Frequency = (float)_database.Frequency;
                            acVoltammetry.Scanrate = (float)_database.ScanRate;
                            acVoltammetry.SineWaveAmplitude = (float)_database.ACPotential;
                            acVoltammetry.StepPotential = (float)_database.PotentialStep;

                            comm.Measure(acVoltammetry);

                            break;

                        case "Chronoamperometry":
                            //run chronoamperometry, not supported currently?

                            break;
                    }

                    #endregion Test Switches

                    comm.Disconnect();
                }
                catch (Exception ex)
                {
                    device.Close();
                    Crashes.TrackError(ex);
                }
            }
        }

        private static bool IsEmulator()
        {
            return "google_sdk".Equals(Build.Product)
                || (Build.Brand.StartsWith("generic") && Build.Device.StartsWith("generic"))
                || Build.Fingerprint.StartsWith("unknown")
                || Build.Hardware.Contains("goldfish")
                || Build.Hardware.Contains("ranchu")
                || Build.Manufacturer.Contains("Genymotion")
                || Build.Model.Contains("Android SDK built for x86")
                || Build.Model.Contains("Emulator")
                || Build.Model.Contains("google_sdk")
                || Build.Fingerprint.StartsWith("generic");
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

        private void Comm_BeginReceiveCurve(object sender, CurveEventArgs e)
        {
            _activeCurve = e.GetCurve();
            _activeCurve.NewDataAdded += _activeCurve_NewDataAdded;
            _activeCurve.Finished += _activeCurve_Finished;
        }

        private void Comm_ReceiveStatus(Object sender, StatusEventArgs e)
        {
            Status status = e.GetStatus();
        }
    }
}