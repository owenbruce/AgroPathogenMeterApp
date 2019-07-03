using Microsoft.AppCenter.Analytics;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class About : ContentPage
    {
        public About()   //Information about the app, APM, and researchers
        {
            Analytics.TrackEvent("About opened");
            InitializeComponent();
        }

        private void OnDarkModeClicked(object sender, EventArgs e)   //Switch between a light mode and a dark mode for the app
        {
            //Change the color theme to be dark
            if (colorChange.Text.ToString() == "Enable Dark Mode")   //Add in capability for the app to change the theme between light and dark
            {
                colorChange.Text = "Enable Light Mode";
            }
            else if (colorChange.Text.ToString() == "Enable Light Mode")
            {
                colorChange.Text = "Enable Dark Mode";
            }
        }
    }
}