using AgroPathogenMeterApp.Models;
using AgroPathogenMeterApp.Views;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgroPathogenMeterApp
{
    [DesignTimeVisible(false)]
    public partial class MasterPage : MasterDetailPage
    {
        private Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();

        public MasterPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Browse:
                        MenuPages.Add(id, new NavigationPage(new MasterPage()));
                        break;
                    case (int)MenuItemType.Bluetooth:
                        MenuPages.Add(id, new NavigationPage(new Bluetooth()));
                        break;

                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new About()));
                        break;
                    case (int)MenuItemType.Instructions:
                        MenuPages.Add(id, new NavigationPage(new instr()));
                        break;
                    case (int)MenuItemType.ViewData:
                        MenuPages.Add(id, new NavigationPage(new dataview()));
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}