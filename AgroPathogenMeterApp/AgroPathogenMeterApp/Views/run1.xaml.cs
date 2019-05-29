using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class run1 : ContentPage   //The first test running menu, will include bluetooth connection and initial instructions
    {
        public run1()
        {
            InitializeComponent();
        }
    }
}