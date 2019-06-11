using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class About : ContentPage
    {
        public About()
        {
            InitializeComponent();
        }

        private void OnDarkModeClicked(object sender, EventArgs e)
        {
            //Change the color theme to be dark
            if (colorChange.Text.ToString() == "Enable Dark Mode")
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