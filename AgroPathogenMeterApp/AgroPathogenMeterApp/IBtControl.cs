using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using AgroPathogenMeterApp.Models;

namespace AgroPathogenMeterApp
{
    public interface IBtControl
    {
        Task<BtDatabase> TestConn();   //Test (and connect to) an APM
        //void Connect();

        String FilePath();   //Get an external file location to store on android
    }
}
