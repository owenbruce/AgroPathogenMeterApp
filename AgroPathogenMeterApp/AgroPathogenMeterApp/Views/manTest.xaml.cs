using AgroPathogenMeterApp.Data;
using AgroPathogenMeterApp.Models;
using System;
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

        private async void OnManTestClicked(object sender, EventArgs e)   //When you click the button to run the manual test
        {
            ScanDatabase _database = new ScanDatabase();
            _database.VoltamType = VoltammetryScan.SelectedItem.ToString();   //Sets the type of voltammetric scan to be run

            #region Parameter Setter
            switch (_database.VoltamType)   //Depending on the type of scan being performed, it sets different values from the same fields
            {
                case "Cyclic Voltammetry":
                    _database.StartingPotential = Convert.ToDouble(Entry1.Text);
                    _database.NegativeVertex = Convert.ToDouble(Entry2.Text);
                    _database.PositiveVertex = Convert.ToDouble(Entry3.Text);
                    _database.PotentialStep = Convert.ToDouble(Entry4.Text);
                    _database.ScanRate = Convert.ToDouble(Entry5.Text);
                    break;

                case "Square Wave Voltammetry":
                    _database.StartingPotential = Convert.ToDouble(Entry1.Text);
                    _database.EndingPotential = Convert.ToDouble(Entry2.Text);
                    _database.PotentialStep = Convert.ToDouble(Entry3.Text);
                    _database.Amplitude = Convert.ToDouble(Entry4.Text);
                    _database.Frequency = Convert.ToDouble(Entry5.Text);
                    break;

                case "Linear Voltammetry":
                    _database.StartingPotential = Convert.ToDouble(Entry1.Text);
                    _database.EndingPotential = Convert.ToDouble(Entry2.Text);
                    _database.PotentialStep = Convert.ToDouble(Entry3.Text);
                    _database.ScanRate = Convert.ToDouble(Entry4.Text);
                    break;

                case "Alternating Current Voltammetry":
                    _database.StartingPotential = Convert.ToDouble(Entry1.Text);
                    _database.EndingPotential = Convert.ToDouble(Entry2.Text);
                    _database.PotentialStep = Convert.ToDouble(Entry3.Text);
                    _database.ACPotential = Convert.ToDouble(Entry4.Text);
                    _database.ScanRate = Convert.ToDouble(Entry5.Text);
                    _database.Frequency = Convert.ToDouble(Entry6.Text);
                    break;

                default:
                    break;
            }
            #endregion
            //await App.Database.SaveScanAsync(_database);

            await Navigation.PushAsync(new testRunning
            {
                BindingContext = _database
            });
        }
    }
}