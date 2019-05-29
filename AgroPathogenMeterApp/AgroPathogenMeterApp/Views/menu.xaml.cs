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

        private async void OnRunTestClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new run1
            {
            });
        }

        private async void OnResultViewClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new dataview
            {
            });
        }

        private async void OnInstrViewClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new instr
            {
            });
        }

        private async void OnDvlpClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new dvlp
            {
            });
        }
    }
}