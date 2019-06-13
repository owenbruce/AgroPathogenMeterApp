using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Run2 : ContentPage
    {
        public Run2()
        {
            InitializeComponent();
        }

        private async void OnNextClicked(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Run3
            {
            });
        }
    }
}