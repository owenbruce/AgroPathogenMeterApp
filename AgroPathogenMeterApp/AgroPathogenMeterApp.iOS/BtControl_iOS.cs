using AgroPathogenMeterApp.iOS;
using AgroPathogenMeterApp.Models;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(BtControl_iOS))]

namespace AgroPathogenMeterApp.iOS
{
    public class BtControl_iOS : IBtControl
    {
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