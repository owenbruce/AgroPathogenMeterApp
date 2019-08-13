using System;
using System.Collections.Generic;
using System.IO;

namespace AgroPathogenMeterApp.Droid
{
    internal class FileHack
    {
        #region DPV

        public List<int> AddLocationsDPV(List<int> locations)   //Adds the locations of the strings which need to be swapped for the scan to process correctly
        {
            locations.Add(279);
            locations.Add(20194);
            return locations;
        }

        public List<string> AddStringLocationsDPV(List<string> stringLocations)   //Adds the (very long) strings which need to be swapped out for the scan to process correctly
        {
            stringLocations.Add("dpv\\r\\nTECHNIQUE = 1\\r" +
                                "\\nNOTES = DPV % 20measurement:% 205 % 20pulses % 20per % 20second % crlfUse % 20PSDummyCell % 20WE_A\\r" +
                                "\\n#Pretreatment and standby\\r\\nE_COND=-6.000E-001\\r\\nT_COND=0.000E+000\\r\\nE_DEP=-5.000E-001\\r\\nT_DEP=0.000E+000\\r" +
                                "\\nT_EQUIL=8.000E+000\\r\\nE_STBY=0.000E+000\\r\\nT_STBY=0.000E+000\\r\\nUSE_STBY=False\\r\\n#Peaks or levels\\r" +
                                "\\nPEAK_HEIGHT_MIN=1.000E-004\\r\\nPEAK_WIDTH_MIN=5.000E-002\\r\\nPEAK_OVERLAP=0.000E+000\\r\\nPEAK_WINDOW=1.000E-001\\r" +
                                "\\nSMOOTH_LEVEL=1\\r\\n#Current ranges\\r\\nIRANGE_MIN=1\\r\\nIRANGE_MAX=3\\r\\nIRANGE_START=3\\r\\n#Auxiliary\\r" +
                                "\\nEXTRA_VALUES_MSK=0\\r\\nUSE_STIRRER=False\\r\\nIRANGE_BIPOT=6\\r\\nE_BIPOT=0.000E+000\\r\\n#Mux Settings\\r" +
                                "\\nMUX_METHOD=-1\\r\\nUSE_MUX_CH=0\\r\\nMUX_SETTINGS=0|False|False|False|False\\r\\nMUX_NO_TIME_RESET=False\\r" +
                                "\\n#Plot view\\r\\nPLOT_BOTTOM=3.386E-006\\r\\nPLOT_LEFT=-5.000E-001\\r\\nPLOT_RIGHT=5.021E-001\\r\\nPLOT_TOP=7.068E-004\\r" +
                                "\\n#Trace analysis\\r\\nCONC_UNIT=mg\\/l|Concentration|c\\r\\n#Polypotentiostat\\r" +
                                "\\nPOLY_E=0.000|1|3|3,0.000|1|3|3,0.000|1|3|3,\\r\\nPOLY_MODE=0,0,0,\\r\\nPOLY_CALIB=0|0|0,0|0|0,0|0|0,\\r" +
                                "\\n#Reference electrode\\r\\nREF_ELECTRODE_NAME=\\r\\nREF_ELECTRODE_OFFSET=0.000E+000\\r\\n#Bipot\\r\\nBIPOT_MODE=0\\r" +
                                "\\nPOLYSTATMODE=0\\r\\nISMAINWE=True\\r\\n#IR Drop Compensation\\r\\nUSE_IR_DROP_COMP=False\\r" +
                                "\\nIR_DROP_COMP_RES=1.000E+000\\r\\n#Options\\r\\nSAVE_ON_DEVICE=True\\r\\nPGSTAT_MODE=8\\r\\nSELECTED_PGSTAT_CHAN=0\\r" +
                                "\\n#Triggering\\r\\nUSE_TRIGGER_EQUIL=False\\r\\nUSE_TRIGGER_START=False\\r\\nUSE_TRIGGER_DELAY=False\\r" +
                                "\\nTRIGGER_VALUE_EQUIL=0\\r\\nTRIGGER_VALUE_START=0\\r\\nTRIGGER_VALUE_DELAY=0\\r\\nTRIGGER_DELAY_PERIOD=5.000E-001\\r" +
                                "\\n#Method overrides\\r\\nOVERRIDES=\\r\\n#Potential method parameters\\r\\nE_BEGIN=-5.000E-001\\r\\nOCP_BEGIN=NaN\\r" +
                                "\\nE_END=5.000E-001\\r\\nOCP_END=NaN\\r\\nE_STEP=5.000E-003\\r\\nVS_PREV_E=False\\r\\n#Scan method parameters\\r" +
                                "\\nANALYTE_NAMES=Cu|||\\r\\nBLANK_TYPE=0\\r\\nCELL_VOLUME=1.000E+001\\r\\nANALYSIS_TYPE=2\\r" +
                                "\\nEPEAK_LEFT=9.999E+003|9.999E+003|9.999E+003|9.999E+003\\r\\nEPEAK_RIGHT=9.999E+003|9.999E+003|9.999E+003|9.999E+003\\r" +
                                "\\nEPEAKS=-5.000E-002|-4.000E-001|-6.000E-001|-1.000E+000\\r\\nAUTO_ANALYTEPEAK=True|True|True|True\\r" +
                                "\\nE_PRETREAT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r\\nT_PRETREAT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r" +
                                "\\nPEAK_VALUE=0\\r\\nSAMPLE_VOLUME=1.000E+001\\r\\nSOL_NR=1|1|1000|1000\\r" +
                                "\\nCONC=1.000E+001|1.000E+001|1.000E+001|1.000E+001\\r\\nTABLE_VALUES_TYPE=2\\r" +
                                "\\nTABLE_VALUES=1.000E+001|1.000E+001|1.000E+001|1.000E+001\\/1.000E+001|1.000E+001|1.000E+001|1.000E+001" +
                                "\\/1.000E+001|1.000E+001|1.000E+001|1.000E+001\\/1.000E+001|1.000E+001|1.000E+001|1.000E+001\\r" +
                                "\\n#Potential method parameters\\r\\nE_BEGIN=-5.000E-001\\r\\nOCP_BEGIN=NaN\\r\\nE_END=5.000E-001\\r\\nOCP_END=NaN\\r" +
                                "\\nE_STEP=5.000E-003\\r\\nVS_PREV_E=False\\r\\n#Pulse method parameters\\r\\nE_PULSE=2.500E-002\\r\\nT_PULSE=7.000E-002\\r" +
                                "\\nSCAN_RATE=2.500E-002\\r\\n\"");

            stringLocations.Add("1\\r\\n#Technique and application\\r\\nMETHOD_ID=dpv\\r\\nTECHNIQUE=1\\r" +
                                "\\nNOTES=DPV%20measurement:%205%20pulses%20per%20second%crlfUse%20PSDummyCell%20WE_A\\r" +
                                "\\n#Pretreatment and standby\\r\\nE_COND=-6.000E-001\\r\\nT_COND=0.000E+000\\r\\nE_DEP=-5.000E-001\\r" +
                                "\\nT_DEP=0.000E+000\\r\\nT_EQUIL=8.000E+000\\r\\nE_STBY=0.000E+000\\r\\nT_STBY=0.000E+000\\r" +
                                "\\nUSE_STBY=False\\r\\n#Peaks or levels\\r\\nPEAK_HEIGHT_MIN=1.000E-004\\r\\nPEAK_WIDTH_MIN=5.000E-002\\r" +
                                "\\nPEAK_OVERLAP=0.000E+000\\r\\nPEAK_WINDOW=1.000E-001\\r\\nSMOOTH_LEVEL=1\\r\\n#Current ranges\\r" +
                                "\\nIRANGE_MIN=1\\r\\nIRANGE_MAX=3\\r\\nIRANGE_START=3\\r\\n#Auxiliary\\r\\nEXTRA_VALUES_MSK=0\\r" +
                                "\\nUSE_STIRRER=False\\r\\nIRANGE_BIPOT=6\\r\\nE_BIPOT=0.000E+000\\r\\n#Mux Settings\\r\\nMUX_METHOD=-1\\r" +
                                "\\nUSE_MUX_CH=0\\r\\nMUX_SETTINGS=0|False|False|False|False\\r\\nMUX_NO_TIME_RESET=False\\r\\n#Plot view\\r" +
                                "\\nPLOT_BOTTOM=1.793E-004\\r\\nPLOT_LEFT=-5.000E-001\\r\\nPLOT_RIGHT=5.000E-001\\r\\nPLOT_TOP=8.293E-004\\r" +
                                "\\n#Trace analysis\\r\\nCONC_UNIT=mg\\/l|Concentration|c\\r\\n#Polypotentiostat\\r" +
                                "\\nPOLY_E=0.000|1|3|3,0.000|1|3|3,0.000|1|3|3,\\r\\nPOLY_MODE=0,0,0,\\r\\nPOLY_CALIB=0|0|0,0|0|0,0|0|0,\\r" +
                                "\\n#Reference electrode\\r\\nREF_ELECTRODE_NAME=\\r\\nREF_ELECTRODE_OFFSET=0.000E+000\\r\\n#Bipot\\r" +
                                "\\nBIPOT_MODE=0\\r\\nPOLYSTATMODE=0\\r\\nISMAINWE=True\\r\\n#IR Drop Compensation\\r" +
                                "\\nUSE_IR_DROP_COMP=False\\r\\nIR_DROP_COMP_RES=1.000E+000\\r\\n#Options\\r\\nSAVE_ON_DEVICE=True\\r" +
                                "\\nPGSTAT_MODE=8\\r\\nSELECTED_PGSTAT_CHAN=0\\r\\n#Triggering\\r\\nUSE_TRIGGER_EQUIL=False\\r" +
                                "\\nUSE_TRIGGER_START=False\\r\\nUSE_TRIGGER_DELAY=False\\r\\nTRIGGER_VALUE_EQUIL=0\\r" +
                                "\\nTRIGGER_VALUE_START=0\\r\\nTRIGGER_VALUE_DELAY=0\\r\\nTRIGGER_DELAY_PERIOD=5.000E-001\\r" +
                                "\\n#Method overrides\\r\\nOVERRIDES=\\r\\n#Potential method parameters\\r\\nE_BEGIN=-5.000E-001\\r" +
                                "\\nOCP_BEGIN=NaN\\r\\nE_END=5.000E-001\\r\\nOCP_END=NaN\\r\\nE_STEP=5.000E-003\\r\\nVS_PREV_E=False\\r" +
                                "\\n#Scan method parameters\\r\\nANALYTE_NAMES=Cu|||\\r\\nBLANK_TYPE=0\\r\\nCELL_VOLUME=1.000E+001\\r" +
                                "\\nANALYSIS_TYPE=2\\r\\nEPEAK_LEFT=9.999E+003|9.999E+003|9.999E+003|9.999E+003\\r" +
                                "\\nEPEAK_RIGHT=9.999E+003|9.999E+003|9.999E+003|9.999E+003\\r" +
                                "\\nEPEAKS=-5.000E-002|-4.000E-001|-6.000E-001|-1.000E+000\\r\\nAUTO_ANALYTEPEAK=True|True|True|True\\r" +
                                "\\nE_PRETREAT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r" +
                                "\\nT_PRETREAT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r\\nPEAK_VALUE=0\\r\\nSAMPLE_VOLUME=1.000E+001\\r" +
                                "\\nSOL_NR=1|1|1000|1000\\r\\nCONC=1.000E+001|1.000E+001|1.000E+001|1.000E+001\\r\\nTABLE_VALUES_TYPE=2\\r" +
                                "\\nTABLE_VALUES=1.000E+001|1.000E+001|1.000E+001|1.000E+001\\/1.000E+001|1.000E+001|1.000E+001|1.000E+001" +
                                "\\/1.000E+001|1.000E+001|1.000E+001|1.000E+001\\/1.000E+001|1.000E+001|1.000E+001|1.000E+001\\r" +
                                "\\n#Potential method parameters\\r\\nE_BEGIN=-5.000E-001\\r\\nOCP_BEGIN=NaN\\r\\nE_END=5.000E-001\\r" +
                                "\\nOCP_END=NaN\\r\\nE_STEP=5.000E-003\\r\\nVS_PREV_E=False\\r\\n#Pulse method parameters\\r" +
                                "\\nE_PULSE=2.500E-002\\r\\nT_PULSE=7.000E-002\\r\\nSCAN_RATE=2.500E-002\\r\\n\",\"curves\":" +
                                "[{\"appearance\":{\"type\":\"PalmSens.Plottables.VisualSettings\",\"autoassigncolor\":true,\"color\":" +
                                "\"-16776961\",\"linewidth\":2,\"symbolsize\":5,\"symboltype\":0,\"symbolfill\":true,\"noline\":false}," +
                                "\"title\":\"DPV I vs E\",\"hash\":" +
                                "[177,188,139,73,20,129,250,105,195,196,95,70,245,239,12,154,17,161,213,205,155,55,84,109,56,100,202,158,100,243,2,11,3,88,246,62,128,29,207,17,159,254,186,228,187,70,60,242]," +
                                "\"type\":\"PalmSens.Plottables.Curve\",\"xaxis\":0,\"yaxis\":0,\"xaxisdataarray\":{\"type\":" +
                                "\"PalmSens.Data.DataArrayPotentials\",\"arraytype\":1,\"description\":\"potential\",\"unit\":{\"type\":" +
                                "\"PalmSens.Units.Volt\",\"s\":\"V\",\"q\":\"Potential\",\"a\":\"E\"}");

            return stringLocations;
        }

        public SimpleMeasurement HackDPV(SimpleMeasurement unHackedMeasurement)
        {
            List<SimpleMeasurement> hackedMeasurements;
            SimpleMeasurement hackedMeasurement;

            String unHackedMeasurementString;
            String hackedMeasurementString = "";

            List<int> locations = new List<int>();
            List<string> stringLocations = new List<string>();

            int startSubstring = 0;

            String file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "dpv.pssession");

            locations = AddLocationsDPV(locations);
            stringLocations = AddStringLocationsDPV(stringLocations);

            SimpleLoadSaveFunctions.SaveMeasurement(unHackedMeasurement, file);

            //Code here for manipulation the file

            using (StreamReader sr = new StreamReader(file))

                unHackedMeasurementString = sr.ReadToEnd();

            for (int i = 0; i < locations.Count; i++)
            {
                hackedMeasurementString += unHackedMeasurementString.Substring(startSubstring, locations[i]) + stringLocations[i];
                startSubstring += locations[i] + stringLocations[i].Length;
            }

            hackedMeasurementString += unHackedMeasurementString.Substring(startSubstring);

            using (StreamWriter writer = File.CreateText(file))

                writer.Write(hackedMeasurementString);

            hackedMeasurements = SimpleLoadSaveFunctions.LoadMeasurements(file);

            hackedMeasurement = hackedMeasurements[0];

            return hackedMeasurement;
        }

        #endregion DPV

        #region SWV

        public List<int> AddLocationsSWV(List<int> locations)   //Adds the locations of the strings which need to be swapped for the scan to process correctly
        {
            locations.Add(272);
            locations.Add(58829);
            locations.Add(489);
            locations.Add(11544);
            return locations;
        }

        public List<string> AddStringLocationsSWV(List<string> stringLocations)   //Adds the (very long) strings which need to be swapped out for the scan to process correctly
        {
            stringLocations.Add("swv\\r\\nTECHNIQUE=2\\r\\nNOTES=\\r\\n#Pretreatment and standby\\r\\nE_COND=0.000E+000\\r" +
                                "\\nT_COND=0.000E+000\\r\\nE_DEP=0.000E+000\\r\\nT_DEP=0.000E+000\\r\\nT_EQUIL=0.000E+000\\r" +
                                "\\nE_STBY=0.000E+000\\r\\nT_STBY=0.000E+000\\r\\nUSE_STBY=False\\r\\n#Peaks or levels\\r" +
                                "\\nPEAK_HEIGHT_MIN=0.000E+000\\r\\nPEAK_WIDTH_MIN=1.000E-001\\r\\nPEAK_OVERLAP=0.000E+000\\r" +
                                "\\nPEAK_WINDOW=1.000E-001\\r\\nSMOOTH_LEVEL=0\\r\\n#Current ranges\\r\\nIRANGE_MIN=2\\r\\nIRANGE_MAX=5\\r" +
                                "\\nIRANGE_START=3\\r\\n#Auxiliary\\r\\nEXTRA_VALUES_MSK=0\\r\\nUSE_STIRRER=False\\r\\nIRANGE_BIPOT=3\\r" +
                                "\\nE_BIPOT=0.000E+000\\r\\n#Mux Settings\\r\\nMUX_METHOD=-1\\r\\nUSE_MUX_CH=0\\r" +
                                "\\nMUX_SETTINGS=0|False|False|False|False\\r\\nMUX_NO_TIME_RESET=False\\r\\n#Plot view\\r" +
                                "\\nPLOT_BOTTOM=-5.000E+001\\r\\nPLOT_LEFT=-6.000E-001\\r\\nPLOT_RIGHT=-9.766E-004\\r\\nPLOT_TOP=-3.000E+001\\r" +
                                "\\n#Polypotentiostat\\r\\nPOLY_E=0.000|3|6|6,0.000|3|6|6,0.000|3|6|6,\\r\\nPOLY_MODE=0,0,0,\\r" +
                                "\\nPOLY_CALIB=0|0|0,0|0|0,0|0|0,\\r\\n#Reference electrode\\r\\nREF_ELECTRODE_NAME=\\r" +
                                "\\nREF_ELECTRODE_OFFSET=0.000E+000\\r\\n#Bipot\\r\\nBIPOT_MODE=0\\r\\nPOLYSTATMODE=0\\r\\nISMAINWE=True\\r" +
                                "\\n#IR Drop Compensation\\r\\nUSE_IR_DROP_COMP=False\\r\\nIR_DROP_COMP_RES=1.000E+000\\r\\n#Options\\r" +
                                "\\nSAVE_ON_DEVICE=True\\r\\nPGSTAT_MODE=8\\r\\nSELECTED_PGSTAT_CHAN=0\\r\\n#Triggering\\r" +
                                "\\nUSE_TRIGGER_EQUIL=False\\r\\nUSE_TRIGGER_START=False\\r\\nUSE_TRIGGER_DELAY=False\\r" +
                                "\\nTRIGGER_VALUE_EQUIL=0\\r\\nTRIGGER_VALUE_START=0\\r\\nTRIGGER_VALUE_DELAY=0\\r" +
                                "\\nTRIGGER_DELAY_PERIOD=5.000E-001\\r\\n#Method overrides\\r\\nOVERRIDES=\\r\\n#Potential method parameters\\r" +
                                "\\nE_BEGIN=0.000E+000\\r\\nOCP_BEGIN=NaN\\r\\nE_END=-6.000E-001\\r\\nOCP_END=NaN\\r\\nE_STEP=1.000E-003\\r" +
                                "\\nVS_PREV_E=False\\r\\n#Scan method parameters\\r\\nANALYTE_NAMES=|||\\r\\nBLANK_TYPE=0\\r" +
                                "\\nCELL_VOLUME=0.000E+000\\r\\nANALYSIS_TYPE=1\\r\\nEPEAK_LEFT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r" +
                                "\\nEPEAK_RIGHT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r" +
                                "\\nEPEAKS=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r\\nAUTO_ANALYTEPEAK=True|True|True|True\\r" +
                                "\\nE_PRETREAT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r" +
                                "\\nT_PRETREAT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r\\nPEAK_VALUE=0\\r\\nSAMPLE_VOLUME=0.000E+000\\r" +
                                "\\nSOL_NR=0|0|0|0\\r\\nCONC=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r\\nTABLE_VALUES_TYPE=2\\r" +
                                "\\nTABLE_VALUES=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\/0.000E+000|0.000E+000|0.000E+000|0.000E+000" +
                                "\\/0.000E+000|0.000E+000|0.000E+000|0.000E+000\\/0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r" +
                                "\\n#Potential method parameters\\r\\nE_BEGIN=0.000E+000\\r\\nOCP_BEGIN=NaN\\r\\nE_END=-6.000E-001\\r" +
                                "\\nOCP_END=NaN\\r\\nE_STEP=1.000E-003\\r\\nVS_PREV_E=False\\r\\n#Square Wave Voltammetry parameters\\r" +
                                "\\nFREQ=6.000E+001\\r\\nE_AMP=2.500E-002\\r\\nMEAS_IFWD_REVERSE=False\\r\\n\"");

            stringLocations.Add("swv\\r\\nTECHNIQUE=2\\r\\nNOTES=\\r\\n#Pretreatment and standby\\r\\nE_COND=0.000E+000\\r\\nT_COND=0.000E+000\\r" +
                                "\\nE_DEP=0.000E+000\\r\\nT_DEP=0.000E+000\\r\\nT_EQUIL=0.000E+000\\r\\nE_STBY=0.000E+000\\r" +
                                "\\nT_STBY=0.000E+000\\r\\nUSE_STBY=False\\r\\n#Peaks or levels\\r\\nPEAK_HEIGHT_MIN=0.000E+000\\r" +
                                "\\nPEAK_WIDTH_MIN=1.000E-001\\r\\nPEAK_OVERLAP=0.000E+000\\r\\nPEAK_WINDOW=1.000E-001\\r\\nSMOOTH_LEVEL=0\\r" +
                                "\\n#Current ranges\\r\\nIRANGE_MIN=2\\r\\nIRANGE_MAX=5\\r\\nIRANGE_START=3\\r\\n#Auxiliary\\r" +
                                "\\nEXTRA_VALUES_MSK=0\\r\\nUSE_STIRRER=False\\r\\nIRANGE_BIPOT=3\\r\\nE_BIPOT=0.000E+000\\r\\n#Mux Settings\\r" +
                                "\\nMUX_METHOD=-1\\r\\nUSE_MUX_CH=0\\r\\nMUX_SETTINGS=0|False|False|False|False\\r\\nMUX_NO_TIME_RESET=False\\r" +
                                "\\n#Plot view\\r\\nPLOT_BOTTOM=-6.266E+001\\r\\nPLOT_LEFT=0.000E+000\\r\\nPLOT_RIGHT=-6.000E-001\\r" +
                                "\\nPLOT_TOP=-1.925E+001\\r\\n#Polypotentiostat\\r\\nPOLY_E=0.000|3|6|6,0.000|3|6|6,0.000|3|6|6,\\r" +
                                "\\nPOLY_MODE=0,0,0,\\r\\nPOLY_CALIB=0|0|0,0|0|0,0|0|0,\\r\\n#Reference electrode\\r\\nREF_ELECTRODE_NAME=\\r" +
                                "\\nREF_ELECTRODE_OFFSET=0.000E+000\\r\\n#Bipot\\r\\nBIPOT_MODE=0\\r\\nPOLYSTATMODE=0\\r\\nISMAINWE=True\\r" +
                                "\\n#IR Drop Compensation\\r\\nUSE_IR_DROP_COMP=False\\r\\nIR_DROP_COMP_RES=1.000E+000\\r\\n#Options\\r" +
                                "\\nSAVE_ON_DEVICE=True\\r\\nPGSTAT_MODE=8\\r\\nSELECTED_PGSTAT_CHAN=0\\r\\n#Triggering\\r" +
                                "\\nUSE_TRIGGER_EQUIL=False\\r\\nUSE_TRIGGER_START=False\\r\\nUSE_TRIGGER_DELAY=False\\r" +
                                "\\nTRIGGER_VALUE_EQUIL=0\\r\\nTRIGGER_VALUE_START=0\\r\\nTRIGGER_VALUE_DELAY=0\\r" +
                                "\\nTRIGGER_DELAY_PERIOD=5.000E-001\\r\\n#Method overrides\\r\\nOVERRIDES=\\r\\n#Potential method parameters\\r" +
                                "\\nE_BEGIN=0.000E+000\\r\\nOCP_BEGIN=NaN\\r\\nE_END=-6.000E-001\\r\\nOCP_END=NaN\\r\\nE_STEP=1.000E-003\\r" +
                                "\\nVS_PREV_E=False\\r\\n#Scan method parameters\\r\\nANALYTE_NAMES=|||\\r\\nBLANK_TYPE=0\\r" +
                                "\\nCELL_VOLUME=0.000E+000\\r\\nANALYSIS_TYPE=1\\r\\nEPEAK_LEFT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r" +
                                "\\nEPEAK_RIGHT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r" +
                                "\\nEPEAKS=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r\\nAUTO_ANALYTEPEAK=True|True|True|True\\r" +
                                "\\nE_PRETREAT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r" +
                                "\\nT_PRETREAT=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r\\nPEAK_VALUE=0\\r\\nSAMPLE_VOLUME=0.000E+000\\r" +
                                "\\nSOL_NR=0|0|0|0\\r\\nCONC=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r\\nTABLE_VALUES_TYPE=2\\r" +
                                "\\nTABLE_VALUES=0.000E+000|0.000E+000|0.000E+000|0.000E+000\\/0.000E+000|0.000E+000|0.000E+000|0.000E+000" +
                                "\\/0.000E+000|0.000E+000|0.000E+000|0.000E+000\\/0.000E+000|0.000E+000|0.000E+000|0.000E+000\\r" +
                                "\\n#Potential method parameters\\r\\nE_BEGIN=0.000E+000\\r\\nOCP_BEGIN=NaN\\r\\nE_END=-6.000E-001\\r" +
                                "\\nOCP_END=NaN\\r\\nE_STEP=1.000E-003\\r\\nVS_PREV_E=False\\r\\n#Square Wave Voltammetry parameters\\r" +
                                "\\nFREQ=6.000E+001\\r\\nE_AMP=2.500E-002\\r\\nMEAS_IFWD_REVERSE=False\\r\\n");

            stringLocations.Add("Potentials\", \"arraytype\":1, \"description\":\"potential\", \"unit\":{\"type\":\"PalmSens.Units.Volt" +
                                "\",\"s\":\"V\",\"q\":\"Potential\",\"a\":\"E");

            stringLocations.Add("VoltageReading");

            return stringLocations;
        }

        public SimpleMeasurement HackSWV(SimpleMeasurement unHackedMeasurement)
        {
            List<SimpleMeasurement> hackedMeasurements;
            SimpleMeasurement hackedMeasurement;

            String unHackedMeasurementString;
            String hackedMeasurementString = "";

            List<int> locations = new List<int>();
            List<string> stringLocations = new List<string>();

            int startSubstring = 0;

            String file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "swv.pssession");

            locations = AddLocationsSWV(locations);
            stringLocations = AddStringLocationsSWV(stringLocations);

            SimpleLoadSaveFunctions.SaveMeasurement(unHackedMeasurement, file);

            //Code here for manipulation the file

            using (StreamReader sr = new StreamReader(file))

                unHackedMeasurementString = sr.ReadToEnd();

            for (int i = 0; i < locations.Count; i++)
            {
                hackedMeasurementString += unHackedMeasurementString.Substring(startSubstring, locations[i]) + stringLocations[i];
                startSubstring += locations[i] + stringLocations[i].Length;
            }

            hackedMeasurementString += unHackedMeasurementString.Substring(startSubstring);

            using (StreamWriter writer = File.CreateText(file))

                writer.Write(hackedMeasurementString);

            hackedMeasurements = SimpleLoadSaveFunctions.LoadMeasurements(file);

            hackedMeasurement = hackedMeasurements[0];

            return hackedMeasurement;
        }

        #endregion SWV
    }
}