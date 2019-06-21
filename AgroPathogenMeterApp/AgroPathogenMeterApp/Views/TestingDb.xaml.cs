using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgroPathogenMeterApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestingDb : ContentPage
    {
        public TestingDb()
        {
            InitializeComponent();
        }

        private async void OnViewResultClicked(object sender, EventArgs e)
        {
            ScanDatabase scan = new ScanDatabase
            {
                IsInfected = false,
                Date = DateTime.Now,
                AmountBacteria = 0,
                ConcentrationBacteria = 0,
                VoltamType = Entry1.Text
            };
            await App.Database.SaveScanAsync(scan);

            await Navigation.PushAsync(new AllData
            {

            });
        }
    }
}