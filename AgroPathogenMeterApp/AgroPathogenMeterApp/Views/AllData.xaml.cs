using AgroPathogenMeterApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllData : ContentPage
    {
        int i;
        public AllData()
        {
            i = 1;
            InitializeComponent();
            GetDatabase();
        }
        private async void GetDatabase()
        {
            ScanDatabase _database = await App.Database.GetScanAsync(i);
            BindingContext = _database;
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            i++;
        }
    }
}