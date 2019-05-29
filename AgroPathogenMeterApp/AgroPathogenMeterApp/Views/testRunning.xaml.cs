﻿using AgroPathogenMeterApp.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class testRunning : ContentPage
    {
        public testRunning()
        {
            InitializeComponent();
        }

        /*
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ScanDatabase _database = await App.Database.GetScanAsync(0);
        }
        */

        private async void OnRunTestClicked(object sender, EventArgs e)
        {
            //Start test on the APM using parameters from _database
            //Once completed, go to finsh test window, and display results
            ScanDatabase _database = await App.Database.GetScanAsync(0);

            await Navigation.PushAsync(new dataview
            {
                BindingContext = _database
            });
        }
    }
}