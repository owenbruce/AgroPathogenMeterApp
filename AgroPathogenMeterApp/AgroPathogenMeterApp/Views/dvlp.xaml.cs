using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class dvlp : ContentPage
    {
        public dvlp()
        {
            InitializeComponent();
        }
        async void OnManualTestClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new manTest { });
        }
        async void OnRawDataClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new rawData { });
        }
    }
}