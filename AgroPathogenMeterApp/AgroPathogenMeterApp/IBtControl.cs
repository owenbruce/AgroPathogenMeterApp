using AgroPathogenMeterApp.Models;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp
{
    public interface IBtControl
    {
        Task<BtDatabase> TestConn();   //Test (and connect to) an APM

        //void Connect(bool simple);

        //string FilePath();   //Get an external file location to store on android
    }
}