﻿using AgroPathogenMeterApp.Models;
using AgroPathogenMeterApp.Data;
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
    public partial class RunFinal : ContentPage
    {
        public RunFinal()
        {
            InitializeComponent();
        }


        private async void OnSaveResultClicked(object sender, EventArgs e)
        {
            //Do stuff
            await Navigation.PushAsync(new dataview{

            });
        }
        private async void OnMoreInfoClicked(object sender, EventArgs e)
        {
            //Do other stuff
            await Navigation.PushAsync(new AllData
            {

            });
        }
    }
}