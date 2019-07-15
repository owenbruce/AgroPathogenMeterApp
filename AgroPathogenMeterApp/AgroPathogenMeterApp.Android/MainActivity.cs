using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using PalmSens;
using PalmSens.Comm;
using System.Security;

namespace AgroPathogenMeterApp.Droid
{
    [Activity(Label = "AgroPathogenMeterApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private BluetoothDeviceReceiver _receiver;
        private PSCommSimpleAndroid psCommSimpleAndroid;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        [SecuritySafeCritical]
        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCenter.Start("72a41ccb-483e-4e33-8786-461a3bc1aaac",
                   typeof(Analytics), typeof(Crashes));

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Xamarin.Forms.DependencyService.Register<IBtControl>();

            base.OnCreate(savedInstanceState);

            psCommSimpleAndroid = FindViewById<PSCommSimpleAndroid>(2131296261);
            psCommSimpleAndroid.ReceiveStatus += _psCommSimpleAndroid_ReceiveStatus;

            Android.Content.Context context = Android.App.Application.Context;
            PalmSens.PSAndroid.Utils.CoreDependencies.Init(context);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            _receiver = new BluetoothDeviceReceiver();

            RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound));

            LoadApplication(new App());
        }

        private void _psCommSimpleAndroid_ReceiveStatus(object sender, PalmSens.Comm.StatusEventArgs e)
        {
            Status status = e.GetStatus(); //Get the PalmSens.Comm.Status instance from the event data
            double potential = status.PotentialReading.Value; //Get the potential
            double currentInRange = status.CurrentReading.ValueInRange; //Get the current expressed inthe active current range
            PalmSens.Comm.ReadingStatus currentStatus = status.CurrentReading.ReadingStatus; //Get the status of the current reading
            CurrentRange cr = status.CurrentReading.CurrentRange; //Get the active current range

            //_txtPotential.Text = $"Potential: {potential.ToString("F3")} V";
            //_txtCurrent.Text = $"Current: {currentInRange.ToString("F3")} * {cr.ToString()}";
        }
    }
}