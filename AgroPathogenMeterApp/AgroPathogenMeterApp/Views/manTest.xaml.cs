using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class manTest : ContentPage   //Allows you to run tests manually with the PalmSens
    {
        public manTest()
        {
            InitializeComponent();
        }

        #region Parameter Range Checkers

        private bool InAmplitudeRange(double value)
        {
            if (value >= 0.001 && value <= 250)
            {
                return true;
            }
            return false;
        }

        private bool InFrequencyRange(double value)
        {
            if (value >= 1 && value <= 1000)
            {
                return true;
            }
            return false;
        }

        private bool InPRange(double value)
        {
            if (Math.Abs(value) <= 10)
            {
                return true;
            }
            return false;
        }

        private bool InScanRateRange(double value)
        {
            if (value >= 0.01 && value <= 500000)
            {
                return true;
            }
            return false;
        }

        private bool InStepRange(double value)
        {
            if (value >= 0.075 && value <= 250)
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

        private bool OnlyDotDash(string value1, string value2, string value3, string value4)
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
            return true;
        }

        private bool OnlyDotDash(string value1, string value2, string value3, string value4, string value5)
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
            return true;
        }

        private bool OnlyDotDash(string value1, string value2, string value3, string value4, string value5, string value6)
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
            return true;
        }

        #endregion Parameter Range Checkers

        private async void OnManTestClicked(object sender, EventArgs e)   //When you click the button to run the manual test
        {
            ScanDatabase _database = new ScanDatabase
            {
                VoltamType = VoltammetryScan.SelectedItem.ToString(),   //Sets the type of voltammetric scan to be run
                IsInfected = false,   //Sets isinfected to false temporarily
                ID = 1
            };
            await App.Database.SaveScanAsync(_database);
            try
            {
                #region Parameter Setter

                switch (_database.VoltamType)   //Depending on the type of scan being performed, it sets different values from the same fields
                {
                    case "Cyclic Voltammetry":
                        if (Entry1.Text.Length >= 1 &&
                            Entry2.Text.Length >= 1 &&
                            Entry3.Text.Length >= 1 &&
                            Entry4.Text.Length >= 1 &&
                            Entry5.Text.Length >= 1)
                        {
                            if (OnlyDotDash(Entry1.Text.ToString(),
                                            Entry2.Text.ToString(),
                                            Entry3.Text.ToString(),
                                            Entry4.Text.ToString(),
                                            Entry5.Text.ToString()))
                            {
                                if (InPRange(Convert.ToDouble(Entry1.Text)) &&
                                    InPRange(Convert.ToDouble(Entry2.Text)) &&
                                    InPRange(Convert.ToDouble(Entry3.Text)) &&
                                    InStepRange(Convert.ToDouble(Entry4.Text)) &&
                                    InScanRateRange(Convert.ToDouble(Entry5.Text)))
                                {
                                    _database.StartingPotential = Convert.ToDouble(Entry1.Text);
                                    _database.NegativeVertex = Convert.ToDouble(Entry2.Text);
                                    _database.PositiveVertex = Convert.ToDouble(Entry3.Text);
                                    _database.PotentialStep = Convert.ToDouble(Entry4.Text);
                                    _database.ScanRate = Convert.ToDouble(Entry5.Text);
                                }
                                else
                                {
                                    await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                                    return;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Warning", "You must enter a number", "OK");
                                return;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                            return;
                            //Setup error loop to get proper values
                        }
                        break;

                    case "Square Wave Voltammetry":
                        if (Entry1.Text.Length >= 1 &&
                            Entry2.Text.Length >= 1 &&
                            Entry3.Text.Length >= 1 &&
                            Entry4.Text.Length >= 1 &&
                            Entry5.Text.Length >= 1)
                        {
                            if (OnlyDotDash(Entry1.Text.ToString(),
                                            Entry2.Text.ToString(),
                                            Entry3.Text.ToString(),
                                            Entry4.Text.ToString(),
                                            Entry5.Text.ToString()))
                            {
                                if (InPRange(Convert.ToDouble(Entry1.Text)) &&
                                    InPRange(Convert.ToDouble(Entry2.Text)) &&
                                    InStepRange(Convert.ToDouble(Entry3.Text)) &&
                                    InAmplitudeRange(Convert.ToDouble(Entry4.Text)) &&
                                    InFrequencyRange(Convert.ToDouble(Entry5.Text)))
                                {
                                    _database.StartingPotential = Convert.ToDouble(Entry1.Text);
                                    _database.EndingPotential = Convert.ToDouble(Entry2.Text);
                                    _database.PotentialStep = Convert.ToDouble(Entry3.Text);
                                    _database.Amplitude = Convert.ToDouble(Entry4.Text);
                                    _database.Frequency = Convert.ToDouble(Entry5.Text);
                                }
                                else
                                {
                                    await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                                    return;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Warning", "You must enter a number", "OK");
                                return;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                            return;
                        }
                        break;

                    case "Linear Voltammetry":
                        if (Entry1.Text.Length >= 1 &&
                            Entry2.Text.Length >= 1 &&
                            Entry3.Text.Length >= 1 &&
                            Entry4.Text.Length >= 1)
                        {
                            if (OnlyDotDash(Entry1.Text.ToString(),
                                            Entry2.Text.ToString(),
                                            Entry3.Text.ToString(),
                                            Entry4.Text.ToString()))
                            {
                                if (InPRange(Convert.ToDouble(Entry1.Text)) &&
                                    InPRange(Convert.ToDouble(Entry2.Text)) &&
                                    InStepRange(Convert.ToDouble(Entry3.Text)) &&
                                    InScanRateRange(Convert.ToDouble(Entry4.Text)))
                                {
                                    _database.StartingPotential = Convert.ToDouble(Entry1.Text);
                                    _database.EndingPotential = Convert.ToDouble(Entry2.Text);
                                    _database.PotentialStep = Convert.ToDouble(Entry3.Text);
                                    _database.ScanRate = Convert.ToDouble(Entry4.Text);
                                }
                                else
                                {
                                    await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                                    return;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Warning", "You must enter a number", "OK");
                                return;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                            return;
                        }
                        break;

                    case "Alternating Current Voltammetry":
                        if (Entry1.Text.Length >= 1 &&
                            Entry2.Text.Length >= 1 &&
                            Entry3.Text.Length >= 1 &&
                            Entry4.Text.Length >= 1 &&
                            Entry5.Text.Length >= 1 &&
                            Entry6.Text.Length >= 1)
                        {
                            if (OnlyDotDash(Entry1.Text.ToString(),
                                            Entry2.Text.ToString(),
                                            Entry3.Text.ToString(),
                                            Entry4.Text.ToString(),
                                            Entry5.Text.ToString(),
                                            Entry6.Text.ToString()))
                            {
                                if (InPRange(Convert.ToDouble(Entry1.Text)) &&
                                    InPRange(Convert.ToDouble(Entry2.Text)) &&
                                    InStepRange(Convert.ToDouble(Entry3.Text)) &&
                                    InPRange(Convert.ToDouble(Entry4.Text)) &&
                                    InScanRateRange(Convert.ToDouble(Entry5.Text)) &&
                                    InFrequencyRange(Convert.ToDouble(Entry6.Text)))
                                {
                                    _database.StartingPotential = Convert.ToDouble(Entry1.Text);
                                    _database.EndingPotential = Convert.ToDouble(Entry2.Text);
                                    _database.PotentialStep = Convert.ToDouble(Entry3.Text);
                                    _database.ACPotential = Convert.ToDouble(Entry4.Text);
                                    _database.ScanRate = Convert.ToDouble(Entry5.Text);
                                    _database.Frequency = Convert.ToDouble(Entry6.Text);
                                }
                                else
                                {
                                    await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                                    return;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Warning", "You must enter a number", "OK");
                                return;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                            return;
                        }
                        break;

                    case "Chronoamperometry":
                        if (Entry1.Text.Length >= 1 &&
                            Entry2.Text.Length >= 1 &&
                            Entry3.Text.Length >= 1)
                        {
                            if (OnlyDotDash(Entry1.Text.ToString(),
                                            Entry2.Text.ToString(),
                                            Entry3.Text.ToString()))
                            {
                                if (InPRange(Convert.ToDouble(Entry1.Text)) &&
                                    InPRange(Convert.ToDouble(Entry2.Text)) &&
                                    InStepRange(Convert.ToDouble(Entry3.Text)))
                                {
                                    _database.AppliedPotential = Convert.ToDouble(Entry1.Text);
                                    _database.TimeInterval = Convert.ToDouble(Entry2.Text);
                                    _database.RunTime = Convert.ToDouble(Entry3.Text);
                                }
                                else
                                {
                                    await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                                    return;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Warning", "You must enter a number", "OK");
                                return;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Warning", "You must fill in all fields with a number within range", "OK");
                            return;
                        }
                        break;

                    default:
                        break;
                }

                #endregion Parameter Setter
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>{
                    { "Entry 1:", Entry1.Text.ToString()  },
                    { "Entry 2:", Entry2.Text.ToString()  },
                    { "Entry 3:", Entry3.Text.ToString()  },
                    { "Entry 4:", Entry4.Text.ToString()  },
                    { "Entry 5:", Entry5.Text.ToString()  },
                    { "Entry 6:", Entry6.Text.ToString()  },
                });
            }
            _database.Date = DateTime.Now;
            await App.Database.SaveScanAsync(_database);

            //DependencyService.Get<BtControl>().connect(_database);  //Runs the test on the APM, need to setup to run async, or move to RunFinal and run async on that page

            await Navigation.PushAsync(new RunFinal
            {
                BindingContext = _database
            });
        }
    }
}