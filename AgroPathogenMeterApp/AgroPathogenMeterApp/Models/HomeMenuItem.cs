namespace AgroPathogenMeterApp.Models
{
    public enum MenuItemType   //Currently unused, may use file, not code
    {
        Browse,
        About
    }

    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}