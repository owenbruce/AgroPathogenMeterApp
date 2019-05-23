using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class menu : ContentPage
    {
        public menu()
        {
            InitializeComponent();
        }
    }
    async void OnRunTestClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NoteEntryPage
        {
            BindingContext = new Note()
        });
    }
}