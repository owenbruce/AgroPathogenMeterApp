using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPage : MasterDetailPage
    {
        public RootPage()  //Root page which holds all other pages
        {
            InitializeComponent();
            MasterBehavior = MasterBehavior.Popover;
        }
    }
}