using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgroPathogenMeterApp.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClearDb : ContentPage
    {
        public ClearDb()
        {
            InitializeComponent();
        }

        private async void OnClearRecentClicked(object sender, EventArgs e)
        {
            var allDb = await App.Database.GetScanDatabasesAsync();
            var _database = await App.Database.GetScanAsync(allDb.Count);
            await App.Database.DeleteScanAsync(_database);
        }

        private async void OnClearAllClicked(object sender, EventArgs e)   //Broken(Don't Use)
        {
            var allDb = await App.Database.GetScanDatabasesAsync();

            for(int i = 0; i < allDb.Count; i++)
            {
                await App.Database.DeleteScanAsync(allDb[i]);
            }
            Scanner scanner = App.Database;
        }
    }
}