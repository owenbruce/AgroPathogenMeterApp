using AgroPathogenMeterApp.Models;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace AgroPathogenMeterApp.Views
{
    [DesignTimeVisible(false)]
    public partial class OptionsPage : ContentPage
    {
        private readonly List<HomeMenuItem> menuItems;
        public OptionsPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Browse, Title="Browse"},
                new HomeMenuItem {Id = MenuItemType.Instructions, Title="Instructions"},
                new HomeMenuItem {Id = MenuItemType.ViewData, Title="View Data"},
                new HomeMenuItem {Id = MenuItemType.Bluetooth, Title="Bluetooth Settings"},
                new HomeMenuItem {Id = MenuItemType.About, Title="About"}
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                {
                    return;
                }

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }

        private MasterPage RootPage { get => Application.Current.MainPage as MasterPage; }
    }
}