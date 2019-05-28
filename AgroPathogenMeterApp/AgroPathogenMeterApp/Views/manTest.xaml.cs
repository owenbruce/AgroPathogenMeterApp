using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgroPathogenMeterApp.Models;
using AgroPathogenMeterApp.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class manTest : ContentPage
    {
        public manTest()
        {
            InitializeComponent();
            BindingContext = new ScanTypes();
        }
        protected override void OnAppearing()
        {

        }

        async void OnManTestClicked(object sender, EventArgs e)
        {
            ScanDatabase _database = new ScanDatabase();
            _database.VoltamType = VoltammetryScan.SelectedItem.ToString();

            switch (_database.VoltamType)
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
            await App.Database.SaveScanAsync(_database);

            await Navigation.PushAsync(new testRunning
            {
                BindingContext = _database
            });
        }
    }
}