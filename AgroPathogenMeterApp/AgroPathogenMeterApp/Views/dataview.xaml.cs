using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class dataview : ContentPage
    {
        public dataview()
        {
            InitializeComponent();
        }

        private async void OnSaveDataClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new menu { });
        }
    }
}