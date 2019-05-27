using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgroPathogenMeterApp.Models;
using AgroPathogenMeterApp.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class manTest : ContentPage
    {
        public manTest()
        {
            InitializeComponent();
            BindingContext = new ScanTypes();
        }
        protected override void OnAppearing()
        {

        }
    }
}