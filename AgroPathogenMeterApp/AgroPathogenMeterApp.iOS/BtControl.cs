using AgroPathogenMeterApp.iOS;
using AgroPathogenMeterApp.Models;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(BtControl))]

namespace AgroPathogenMeterApp.iOS
{
    public class BtControl : IBtControl
    {
        public BtControl()
        {
        }

        public async Task<BtDatabase> TestConn()
        {
            BtDatabase _database = null;
            return _database;
        }

        public async void Connect(bool simple)
        {
        }

        public string FilePath()
        {
            return "";
        }
    }
}