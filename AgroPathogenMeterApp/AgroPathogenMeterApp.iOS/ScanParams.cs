using AgroPathogenMeterApp.Models;
using PalmSens.Techniques;

namespace AgroPathogenMeterApp.iOS
{
    internal class ScanParams
    {
        public LinearSweep LinSweep(ScanDatabase _database)   //Set the parameters for a linear sweep scan
        {
            LinearSweep linearSweep = new LinearSweep
            {
                BeginPotential = (float)_database.StartingPotential,
                EndPotential = (float)_database.EndingPotential,
                Scanrate = (float)_database.ScanRate,
                StepPotential = (float)_database.PotentialStep    //Add in additional required values once determined
            };

            return linearSweep;
        }

        public SquareWave SWV(ScanDatabase _database)   //Set the parameters for a square wave scan
        {
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

        public CyclicVoltammetry CV(ScanDatabase _database)   //Set the parameters for a cyclic scan
        {
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

        public ACVoltammetry ACV(ScanDatabase _database)   //Set the parameters for an alternating current scan
        {
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