using AgroPathogenMeterApp.Data;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.IO;
using Xamarin.Forms;

namespace AgroPathogenMeterApp
{
    public partial class App : Application   //Contains all information needed by the app
    {
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        public static string AzureBackendUrl = "http://localhost:5000";

        public static bool UseMockDataStore = true;
        private static Scanner scanner;
        public static NavigationPage NavigationPage { get; private set; }

        

        public App()
        {
            InitializeComponent();
            MainPage = new MasterPage();
        }

        public static Scanner Database   //Creates the main database that will be used
        {
            get
            {
                if (scanner == null)
                {
                    scanner = new Scanner(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
                }
                return scanner;
            }
        }
        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void OnSleep()
        {

            // Handle when your app sleeps

        }

        protected override void OnStart()
        {
            AppCenter.Start("android=72a41ccb-483e-4e33-8786-461a3bc1aaac;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={b2f34a3d-c5ba-4523-92f2-1321a1a55616}",
                  typeof(Analytics), typeof(Crashes));
            // Handle when your app starts
        }
    }
}