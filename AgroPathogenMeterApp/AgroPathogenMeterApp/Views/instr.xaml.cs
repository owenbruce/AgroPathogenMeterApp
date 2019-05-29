using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class instr : ContentPage    //Opens page which shows instructions on how to use the APM
    {
        public instr()
        {
            InitializeComponent();
        }
    }
}