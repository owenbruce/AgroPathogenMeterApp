namespace AgroPathogenMeterApp.Models
{
    public class Item   //Example item for azure storage, may use if sqlite doesn't work
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }
}