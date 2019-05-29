using AgroPathogenMeterApp.Models;

namespace AgroPathogenMeterApp.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel   //More item viewmodel
    {
        public Item Item { get; set; }

        public ItemDetailViewModel(Item item = null)
        {
            Title = item?.Text;
            Item = item;
        }
    }
}