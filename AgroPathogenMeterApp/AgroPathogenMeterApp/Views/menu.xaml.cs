﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : ContentPage   //Main menu page with user options and hidden developer button in lower right
    {
        public Menu()
        {
            InitializeComponent();
        }

        private async void OnDvlpClicked(object sender, EventArgs e)   //Opens developer menu (Button is not visible, but is in the lower right corner)
        {
            await Navigation.PushAsync(new dvlp
            {
            });
        }

        private async void OnInstrViewClicked(object sender, EventArgs e)   //Opens the instructions menu
        {
            await Navigation.PushAsync(new instr
            {
            });
        }

        private async void OnResultViewClicked(object sender, EventArgs e)   //Opens the dataview menu to view previous data
        {
            await Navigation.PushAsync(new dataview
            {
            });
        }

        private async void OnRunTestClicked(object sender, EventArgs e)   //Opens the first test running menu
        {
            await Navigation.PushAsync(new run1
            {
            });
        }
    }
}