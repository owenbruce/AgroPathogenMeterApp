namespace AgroPathogenMeterApp.Models
{
    public enum MenuItemType   //Currently unused, may use file, not code
    {
        Browse,
        Bluetooth,
        About,
        Instructions,
        ViewData
    }

    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}