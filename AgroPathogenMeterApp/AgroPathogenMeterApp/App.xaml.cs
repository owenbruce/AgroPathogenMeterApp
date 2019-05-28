using AgroPathogenMeterApp.Views;
using System;
using System.IO;
using Xamarin.Forms;
using AgroPathogenMeterApp.Data;


namespace AgroPathogenMeterApp
{
    public partial class App : Application
    {

        static Scanner scanner;
        public static Scanner Database
        {
            get
            {
                if(Database == null)
                {
                    scanner = new Scanner(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
                }
                return scanner;
            }
        }
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        public static string AzureBackendUrl = "http://localhost:5000";
        public static bool UseMockDataStore = true;

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new menu());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
