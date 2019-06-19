﻿using AgroPathogenMeterApp.Droid;
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
        //private static Scanner scanner;
        /*
        public Scanner getScan()
        {
        }
        */

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
                            SquareWave squareWave = new SquareWave
                            {
                                BeginPotential = (float)_database.StartingPotential,
                                EndPotential = (float)_database.EndingPotential,
                                Frequency = (float)_database.Frequency,
                                PulseAmplitude = (float)_database.Amplitude,
                                StepPotential = (float)_database.PotentialStep
                            };

                            comm.Measure(squareWave);

                            break;

                        case "Linear Voltammetry":
                            LinearSweep linearSweep = new LinearSweep
                            {
                                BeginPotential = (float)_database.StartingPotential,
                                EndPotential = (float)_database.EndingPotential,
                                Scanrate = (float)_database.ScanRate,
                                StepPotential = (float)_database.PotentialStep
                            };

                            comm.Measure(linearSweep);

                            break;

                        case "Cyclic Voltammetry":
                            CyclicVoltammetry cyclicVoltammetry = new CyclicVoltammetry
                            {
                                BeginPotential = (float)_database.StartingPotential,
                                nScans = 1,
                                Scanrate = (float)_database.ScanRate,
                                StepPotential = (float)_database.PotentialStep,
                                Vtx1Potential = (float)_database.NegativeVertex,
                                Vtx2Potential = (float)_database.PositiveVertex
                            };

                            comm.Measure(cyclicVoltammetry);

                            break;

                        case "Alternating Current Voltammetry":
                            ACVoltammetry acVoltammetry = new ACVoltammetry
                            {
                                BeginPotential = (float)_database.StartingPotential,
                                EndPotential = (float)_database.EndingPotential,
                                Frequency = (float)_database.Frequency,
                                Scanrate = (float)_database.ScanRate,
                                SineWaveAmplitude = (float)_database.ACPotential,
                                StepPotential = (float)_database.PotentialStep
                            };

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