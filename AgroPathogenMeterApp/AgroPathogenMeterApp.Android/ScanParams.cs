using AgroPathogenMeterApp.Models;
using PalmSens.Techniques;

namespace AgroPathogenMeterApp.Droid
{
    internal class ScanParams
    {
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
            acVoltammetry.Technique = 4;
            return acVoltammetry;
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
            cyclicVoltammetry.Technique = 5;
            return cyclicVoltammetry;
        }

        public DifferentialPulse DPV(ScanDatabase _database)
        {
            DifferentialPulse differentialPulse = new DifferentialPulse
            {
                EquilibrationTime = (float)_database.EquilTime,
                BeginPotential = (float)_database.StartingPotential,
                EndPotential = (float)_database.EndingPotential,
                StepPotential = (float)_database.PotentialStep,
                PulsePotential = (float)_database.PotentialPulse,
                PulseTime = (float)_database.TimePulse,
                Scanrate = (float)_database.ScanRate
            };
            differentialPulse.Technique = 1;
            return differentialPulse;
        }

        public LinearSweep LinSweep(ScanDatabase _database)   //Set the parameters for a linear sweep scan
        {
            LinearSweep linearSweep = new LinearSweep
            {
                BeginPotential = (float)_database.StartingPotential,
                EndPotential = (float)_database.EndingPotential,
                Scanrate = (float)_database.ScanRate,
                StepPotential = (float)_database.PotentialStep    //Add in additional required values once determined
            };
            linearSweep.Technique = 0;
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
            squareWave.Technique = 2;
            return squareWave;
        }
    }
}