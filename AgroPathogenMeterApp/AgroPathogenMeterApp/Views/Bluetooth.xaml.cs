﻿using Xamarin.Forms;
using System;
using PalmSens.Comm;
using PalmSens.Devices;
using PalmSens.PSAndroid.Comm;
using Xamarin.Forms.Xaml;
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

        private async void OnBtClicked(Object sender, EventArgs e)
        {
            /*
            PalmSens.Devices.Device[] devices = new PalmSens.Devices.Device[0];
            DeviceDiscoverer deviceDiscoverer = new DeviceDiscoverer(Context);
            devices = (await deviceDiscoverer.Discover(true, true)).ToArray();
            deviceDiscoverer.Dispose();

            CommManager comm;
            PalmSens.Devices.Device device = devices[0];
            try
            {
                device.Open();
                comm = new CommManager(device, 5000);
            }
            catch (Exception ex)
            {
                device.Close();
                Crashes.TrackError(ex);
            }
            */
        }
    }
}