using System;
using System.Collections.Generic;
using AgroPathogenMeterApp.Models;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RunFinal : ContentPage
    {
        public RunFinal()
        {
            InitializeComponent();
        }

        private async void OnMoreInfoClicked(object sender, EventArgs e)
        {
            //Do other stuff
            await Navigation.PushAsync(new AllData
            {
            });
        }

        private async void OnSaveResultClicked(object sender, EventArgs e)
        {
            List<ScanDatabase> allDb = await App.Database.GetScanDatabasesAsync();
            //Do stuff
            try
            {
                ScanDatabase _database;
                    try
                    {
                        _database = await App.Database.GetScanAsync(allDb.Count);
                        _database.Date = DateTime.Now;
                        await App.Database.SaveScanAsync(_database);
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex, new Dictionary<string, string>
                        {
                            {"allDb.Count:" , allDb.Count.ToString()}
                        }
                        );
                    }
            }
            catch(Exception ex)
            {

                Crashes.TrackError(ex, new Dictionary<string, string>
                {
                    {"allDb.Count:" , allDb.Count.ToString()}
                }
                );
            }
            
            

            await Navigation.PushAsync(new dataview
            {
            });
        }
    }
}