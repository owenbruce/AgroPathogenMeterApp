using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Security;

namespace AgroPathogenMeterApp.Droid
{
    [Activity(Label = "AgroPathogenMeterApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private BluetoothDeviceReceiver _receiver;

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

            Android.Content.Context context = Android.App.Application.Context;
            PalmSens.PSAndroid.Utils.CoreDependencies.Init(context);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            const int locationPermissionsRequestCode = 1000;

            var locationPermissions = new[]
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };

            var coarseLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);

            var fineLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);

            if (coarseLocationPermissionGranted == Permission.Denied ||
                fineLocationPermissionGranted == Permission.Denied)
            {
                ActivityCompat.RequestPermissions(this, locationPermissions, locationPermissionsRequestCode);
            }

            _receiver = new BluetoothDeviceReceiver();

            RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound));

            LoadApplication(new App());
        }
    }
}