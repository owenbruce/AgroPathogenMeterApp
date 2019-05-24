using System;

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
        async void OnRunTestClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new run1
            {
                
            });
        }
        async void OnResultViewClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new dataview
            {
               
            });
        }
        async void OnInstrViewClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new instr
            {
                
            });
        }
        async void OnDvlpClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new dvlp
            {

            });
        }
    }

}