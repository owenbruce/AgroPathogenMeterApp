﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Sentry;
using System;
using System.Collections.Generic;
using System.Security;

namespace AgroPathogenMeterApp.Droid
{
    [Activity(Label = "AgroPathogenMeterApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        [SecuritySafeCritical]
        protected override void OnCreate(Bundle savedInstanceState)
        {

            CheckCrash();
            
            AppCenter.Start("72a41ccb-483e-4e33-8786-461a3bc1aaac",
                   typeof(Analytics), typeof(Crashes));

                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                base.OnCreate(savedInstanceState);

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
                LoadApplication(new App());
            
        }

        private async void CheckCrash()
        {
            ErrorReport crashReport = await Crashes.GetLastSessionCrashReportAsync();

            Exception ex = new Exception("Last Crash Reason");

            Crashes.TrackError(ex, new Dictionary<string, string>{
                    { "Crash Reason", crashReport.ToString()  },
                });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}