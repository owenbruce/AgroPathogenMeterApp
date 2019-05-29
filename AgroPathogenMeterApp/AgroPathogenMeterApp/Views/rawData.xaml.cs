using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class rawData : ContentPage   //Allows for developers to view the raw data
    {
        public rawData()
        {
            InitializeComponent();
        }
    }
}