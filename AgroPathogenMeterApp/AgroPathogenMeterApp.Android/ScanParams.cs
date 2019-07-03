using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgroPathogenMeterApp.Data;
using PalmSens;
using PalmSens.Techniques;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using AgroPathogenMeterApp.Models;

namespace AgroPathogenMeterApp.Droid
{
    class ScanParams
    {
        private ScanDatabase _database = null;

        private async void SetDatabase()
        {
            var allDb = await App.Database.GetScanDatabasesAsync();
            var _database = await App.Database.GetScanAsync(allDb.Count);
        }

        public LinearSweep LinSweep()   //Set the parameters for a linear sweep scan
        {
            if (_database == null)
            {
                SetDatabase();
            }

            LinearSweep linearSweep = new LinearSweep
            {
                BeginPotential = (float)_database.StartingPotential,
                EndPotential = (float)_database.EndingPotential,
                Scanrate = (float)_database.ScanRate,
                StepPotential = (float)_database.PotentialStep    //Add in additional required values once determined
            };
            

            return linearSweep;
        }
        public SquareWave SWV()   //Set the parameters for a square wave scan
        {
            if (_database == null)
            {
                SetDatabase();
            }

            SquareWave squareWave = new SquareWave
            {
                BeginPotential = (float)_database.StartingPotential,
                EndPotential = (float)_database.EndingPotential,
                StepPotential = (float)_database.PotentialStep,
                PulseAmplitude = (float)_database.Amplitude,
                Frequency = (float)_database.Frequency
            };

            return squareWave;
        }

        public CyclicVoltammetry CV()   //Set the parameters for a cyclic scan
        {
            if (_database == null)
            {
                SetDatabase();
            }
            CyclicVoltammetry cyclicVoltammetry = new CyclicVoltammetry
            {
                BeginPotential = (float)_database.StartingPotential,
                Vtx1Potential = (float)_database.NegativeVertex,
                Vtx2Potential = (float)_database.PositiveVertex,
                StepPotential = (float)_database.PotentialStep,
                Scanrate = (float)_database.ScanRate
            };

            return cyclicVoltammetry;
        }
        public ACVoltammetry ACV()   //Set the parameters for an alternating current scan
        {
            if(_database == null)
            {
                SetDatabase();
            }
            ACVoltammetry acVoltammetry = new ACVoltammetry
            {
                BeginPotential = (float)_database.StartingPotential,
                EndPotential = (float)_database.EndingPotential,
                StepPotential = (float)_database.PotentialStep,
                SineWaveAmplitude = (float)_database.ACPotential,
                Scanrate = (float)_database.ScanRate,
                Frequency = (float)_database.Frequency
            };

            return acVoltammetry;
        }
    }
}