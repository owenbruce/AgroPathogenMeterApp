using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Bluetooth : ContentPage
    {
        public Bluetooth()
        {
            InitializeComponent();
        }

        private void OnBtClicked(Object sender, EventArgs e)
        {
            DependencyService.Get<BtControl>().connect();
        }
    }
}