using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AgroPathogenMeterApp.Droid
{
    class FileHack
    {
        public List<int> AddLocations(List<int> locations)
        {

            return locations;
        }

        public List<string> AddStringLocations(List<string> stringLocations)
        {
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

            String file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "dpv.pssession");

            locations = AddLocations(locations);
            stringLocations = AddStringLocations(stringLocations);

            SimpleLoadSaveFunctions.SaveMeasurement(unHackedMeasurement, file);

            //Code here for manipulation the file

            using (StreamReader sr = new StreamReader(file))

                unHackedMeasurementString = sr.ReadToEnd();

            for(int i = 0; i<locations.Count; i++)
            {
                hackedMeasurementString += unHackedMeasurementString.Substring(0, locations[i]) + stringLocations[i];
            }

            using (StreamWriter writer = File.CreateText(file))

                writer.Write(hackedMeasurementString);

            hackedMeasurements = SimpleLoadSaveFunctions.LoadMeasurements(file);

            hackedMeasurement = hackedMeasurements[0];

            return hackedMeasurement;
        }
    }
}