using AgroPathogenMeterApp.Data;
using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class manTest : ContentPage   //Allows you to run tests manually with the PalmSens
    {
        public manTest()
        {
            Analytics.TrackEvent("Manual Test Opened");
            InitializeComponent();
        }

        #region Parameter Range Checkers

        private bool InAmplitudeRange(double value)   //Check if the chosen amplitude is within the possibe range of the potentiostat
        {
            if (value >= 0.001 && value <= 0.25)
            {
                return true;
            }
            return false;
        }

        private bool InFrequencyRange(double value)   //CHeck if the chosen frequency is within the possible range of the potentiostat
        {
            if (value >= 1 && value <= 2000)
            {
                return true;
            }
            return false;
        }

        private bool InPRange(double value)   //Check if the starting or ending potential is within the possible range of the potentiostat
        {
            if (Math.Abs(value) <= 5)
            {
                return true;
            }
            return false;
        }

        private bool InScanRateRange(double value)   //Check if the scan rate is within the possible range of the potentiostat
        {
            if (value >= 0.01 && value <= 500000)
            {
                return true;
            }
            return false;
        }

        private bool InStepRange(double value)   //Check if the potential step is within the possible range of the potentiostat
        {
            if (value >= 0.000076 && value <= 0.25)
            {
                return true;
            }
            return false;
        }

        private bool OnlyDotDash(string value1, string value2, string value3)
        {
            if (value1.Equals(".") ||
                value2.Equals(".") ||
                value3.Equals("."))
            {
                return false;
            }
            else if (value1.Equals("-") ||
                     value2.Equals("-") ||
                     value3.Equals("-"))
            {
                return false;
            }
            return true;
        }
        private bool OnlyDotDash(string value1,
                                 string value2,
                                 string value3,
                                 string value4)   //Checks to make sure there is a number entered
        {
            if (value1.Equals(".") ||
                value2.Equals(".") ||
                value3.Equals(".") ||
                value4.Equals("."))
            {
                return false;
            }
            else if (value1.Equals("-") ||
                     value2.Equals("-") ||
                     value3.Equals("-") ||
                     value4.Equals("-"))
            {
                return false;
            }
            else if (value1.Equals("-.") ||
                     value2.Equals("-.") ||
                     value3.Equals("-.") ||
                     value4.Equals("-."))
            {
                return false;
            }
            return true;
        }

        private bool OnlyDotDash(string value1,
                                 string value2,
                                 string value3,
                                 string value4,
                                 string value5)   //Checks to make sure there is a number entered
        {
            if (value1.Equals(".") ||
                value2.Equals(".") ||
                value3.Equals(".") ||
                value4.Equals(".") ||
                value5.Equals("."))
            {
                return false;
            }
            else if (value1.Equals("-") ||
                     value2.Equals("-") ||
                     value3.Equals("-") ||
                     value4.Equals("-") ||
                     value5.Equals("-"))
            {
                return false;
            }
            else if (value1.Equals("-.") ||
                     value2.Equals("-.") ||
                     value3.Equals("-.") ||
                     value4.Equals("-.") ||
                     value5.Equals("-."))
            {
                return false;
            }
            return true;
        }

        private bool OnlyDotDash(string value1,
                                 string value2,
                                 string value3,
                                 string value4,
                                 string value5,
                                 string value6)  //Checks to make sure there's a value entered
        {
            if (value1.Equals(".") ||
                value2.Equals(".") ||
                value3.Equals(".") ||
                value4.Equals(".") ||
                value5.Equals(".") ||
                value6.Equals("."))
            {
                return false;
            }
            else if (value1.Equals("-") ||
                     value2.Equals("-") ||
                     value3.Equals("-") ||
                     value4.Equals("-") ||
                     value5.Equals("-") ||
                     value6.Equals("-"))
            {
                return false;
            }
            else if (value1.Equals("-.") ||
                     value2.Equals("-.") ||
                     value3.Equals("-.") ||
                     value4.Equals("-.") ||
                     value5.Equals("-.") ||
                     value6.Equals("-."))
            {
                return false;
            }
            return true;
        }

        #endregion Parameter Range Checkers

        #region Parameter Setter

        private async Task<ScanDatabase> ACV(ScanDatabase Scan)
        {
            if (Entry1.Text.Length >= 1 &&
                            Entry2.Text.Length >= 1 &&
                            Entry3.Text.Length >= 1 &&
                            Entry4.Text.Length >= 1 &&
                            Entry5.Text.Length >= 1 &&
                            Entry6.Text.Length >= 1)   //If there is an entry in all of the requried fields
            {
                if (OnlyDotDash(Entry1.Text,
                                Entry2.Text,
                                Entry3.Text,
                                Entry4.Text,
                                Entry5.Text,
                                Entry6.Text))   //If all of them have numbers
                {
                    if (InPRange(Convert.ToDouble(Entry1.Text)) &&
                        InPRange(Convert.ToDouble(Entry2.Text)) &&
                        InStepRange(Convert.ToDouble(Entry3.Text)) &&
                        InPRange(Convert.ToDouble(Entry4.Text)) &&
                        InScanRateRange(Convert.ToDouble(Entry5.Text)) &&
                        InFrequencyRange(Convert.ToDouble(Entry6.Text)))   //If they are all in range
                    {
                        Scan.StartingPotential = Convert.ToDouble(Entry1.Text);   //Set parameters in the database based on entered values
                        Scan.EndingPotential = Convert.ToDouble(Entry2.Text);
                        Scan.PotentialStep = Convert.ToDouble(Entry3.Text);
                        Scan.ACPotential = Convert.ToDouble(Entry4.Text);
                        Scan.ScanRate = Convert.ToDouble(Entry5.Text);
                        Scan.Frequency = Convert.ToDouble(Entry6.Text);
                    }
                    else
                    {
                        await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                        return null;
                    }
                }
                else
                {
                    await DisplayAlert("Warning", "You must enter a number", "OK");
                    return null;
                }
            }
            else
            {
                await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                return null;
            }
            return Scan;
        }

        private async Task<ScanDatabase> LinearVoltammetry(ScanDatabase Scan)
        {
            if (Entry1.Text.Length >= 1 &&
                            Entry2.Text.Length >= 1 &&
                            Entry3.Text.Length >= 1 &&
                            Entry4.Text.Length >= 1)  //If there is an entry in all of the required fields
            {
                if (OnlyDotDash(Entry1.Text,
                                Entry2.Text,
                                Entry3.Text,
                                Entry4.Text))   //If all of them have numbers
                {
                    if (InPRange(Convert.ToDouble(Entry1.Text)) &&
                        InPRange(Convert.ToDouble(Entry2.Text)) &&
                        InStepRange(Convert.ToDouble(Entry3.Text)) &&
                        InScanRateRange(Convert.ToDouble(Entry4.Text)))   //If all of them are in range
                    {
                        Scan.StartingPotential = Convert.ToDouble(Entry1.Text);   //Set paramters in the database based on entered values
                        Scan.EndingPotential = Convert.ToDouble(Entry2.Text);
                        Scan.PotentialStep = Convert.ToDouble(Entry3.Text);
                        Scan.ScanRate = Convert.ToDouble(Entry4.Text);
                    }
                    else
                    {
                        await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                        return null;
                    }
                }
                else
                {
                    await DisplayAlert("Warning", "You must enter a number", "OK");
                    return null;
                }
            }
            else
            {
                await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                return null;
            }
            return Scan;
        }

        private async Task<ScanDatabase> SWV(ScanDatabase Scan)
        {
            if (Entry1.Text.Length >= 1 &&
                            Entry2.Text.Length >= 1 &&
                            Entry3.Text.Length >= 1 &&
                            Entry4.Text.Length >= 1 &&
                            Entry5.Text.Length >= 1)   //If there is an entry in all of the required fields
            {
                if (OnlyDotDash(Entry1.Text,
                                Entry2.Text,
                                Entry3.Text,
                                Entry4.Text,
                                Entry5.Text))   //If all of them have numbers
                {
                    if (InPRange(Convert.ToDouble(Entry1.Text)) &&
                        InPRange(Convert.ToDouble(Entry2.Text)) &&
                        InStepRange(Convert.ToDouble(Entry3.Text)) &&
                        InAmplitudeRange(Convert.ToDouble(Entry4.Text)) &&
                        InFrequencyRange(Convert.ToDouble(Entry5.Text)))   //If all of them are in range
                    {
                        Scan.StartingPotential = Convert.ToDouble(Entry1.Text);   //Set parameters in the databse based on entered values
                        Scan.EndingPotential = Convert.ToDouble(Entry2.Text);
                        Scan.PotentialStep = Convert.ToDouble(Entry3.Text);
                        Scan.Amplitude = Convert.ToDouble(Entry4.Text);
                        Scan.Frequency = Convert.ToDouble(Entry5.Text);
                    }
                    else
                    {
                        await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                        return null;
                    }
                }
                else
                {
                    await DisplayAlert("Warning", "You must enter a number", "OK");
                    return null;
                }
            }
            else
            {
                await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                return null;
            }
            return Scan;
        }

        private async Task<ScanDatabase> CyclicVotammetry(ScanDatabase Scan)
        {
            if (Entry1.Text.Length >= 1 &&
                            Entry2.Text.Length >= 1 &&
                            Entry3.Text.Length >= 1 &&
                            Entry4.Text.Length >= 1 &&
                            Entry5.Text.Length >= 1)   //If there is an entry in all of the required fields
            {
                if (OnlyDotDash(Entry1.Text,
                                Entry2.Text,
                                Entry3.Text,
                                Entry4.Text,
                                Entry5.Text))   //If all of them have numbers
                {
                    if (InPRange(Convert.ToDouble(Entry1.Text)) &&
                        InPRange(Convert.ToDouble(Entry2.Text)) &&
                        InPRange(Convert.ToDouble(Entry3.Text)) &&
                        InStepRange(Convert.ToDouble(Entry4.Text)) &&
                        InScanRateRange(Convert.ToDouble(Entry5.Text)))   //If all of them are in the correct range
                    {
                        Scan.StartingPotential = Convert.ToDouble(Entry1.Text);   //Set parameters in the database based on entered values
                        Scan.NegativeVertex = Convert.ToDouble(Entry2.Text);
                        Scan.PositiveVertex = Convert.ToDouble(Entry3.Text);
                        Scan.PotentialStep = Convert.ToDouble(Entry4.Text);
                        Scan.ScanRate = Convert.ToDouble(Entry5.Text);
                    }
                    else
                    {
                        await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                        return null;
                    }
                }
                else
                {
                    await DisplayAlert("Warning", "You must enter a number", "OK");
                    return null;
                }
            }
            else
            {
                await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                return null;
                //Setup error loop to get proper values
            }
            return Scan;
        }

        #endregion Parameter Setter

        private async void OnManTestClicked(object sender, EventArgs e)   //When you click the button to run the manual test
        {
            Scanner scanner = App.Database;
            var Scan = new ScanDatabase
            {
                VoltamType = VoltammetryScan.SelectedItem.ToString(),   //Sets the type of voltammetric scan to be run
                IsInfected = false   //Sets isinfected to false temporarily
            };
            try
            {
                switch (Scan.VoltamType)   //Depending on the type of scan being performed, it sets different values from the same fields
                {
                    case "Cyclic Voltammetry":   //If a cyclic voltammetry scan is wanted
                        Scan = await CyclicVotammetry(Scan);
                        if (Scan == null) return;
                        break;

                    case "Square Wave Voltammetry":   //If a squre wave voltammetric scan is wanted
                        Scan = await SWV(Scan);
                        if (Scan == null) return;
                        break;

                    case "Linear Voltammetry":   //If a Linear Voltammetric scan is wanted
                        Scan = await LinearVoltammetry(Scan);
                        if (Scan == null) return;
                        break;

                    case "Alternating Current Voltammetry":   //If an alternating current voltammetric scan is wanted
                        Scan = await ACV(Scan);
                        if (Scan == null) return;
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>{
                    { "Entry 1:", Entry1.Text  },
                    { "Entry 2:", Entry2.Text  },
                    { "Entry 3:", Entry3.Text  },
                    { "Entry 4:", Entry4.Text  },
                    { "Entry 5:", Entry5.Text  },
                    { "Entry 6:", Entry6.Text  },
                });
            }

            Scan.Date = DateTime.Now;   //Set the date/time of the scan
            await scanner.SaveScanAsync(Scan);

            //File.Copy(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "notes.db3"), DependencyService.Get<BtControl>().FilePath());

            await DependencyService.Get<IBtControl>().Connect(1, false, false, true, false);  //Runs the test on the APM, need to setup to run async, or move to RunFinal and run async on that page

            await Navigation.PushAsync(new RunFinal
            {
            });
        }
    }
}