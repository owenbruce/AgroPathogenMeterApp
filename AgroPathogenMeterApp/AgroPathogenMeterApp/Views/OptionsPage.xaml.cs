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

        public OptionsPage()   //Hamburger menu contains menus
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>   //Initialize the choices in the hamburger menu
            {
                new HomeMenuItem {Id = MenuItemType.Browse, Title="Browse"},
                new HomeMenuItem {Id = MenuItemType.Instructions, Title="Instructions"},
                new HomeMenuItem {Id = MenuItemType.ViewData, Title="View Data"},
                new HomeMenuItem {Id = MenuItemType.Bluetooth, Title="Bluetooth Settings"},
                new HomeMenuItem {Id = MenuItemType.About, Title="About"}
            };

            ListViewMenu.ItemsSource = menuItems;   //Set the items in the menu to be the initialized ones above

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>   //Opend the required screen based upon the selected item
            {
                if (e.SelectedItem == null)
                {
                    return;
                }

                int id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }

        private MasterPage RootPage { get => Application.Current.MainPage as MasterPage; }
    }
}