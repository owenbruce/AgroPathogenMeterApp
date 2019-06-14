using Xamarin.Forms;
using System;
using PalmSens.Comm;
using PalmSens.Devices;
using PalmSens.PSAndroid.Comm;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Microsoft.AppCenter.Crashes;

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