using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using AgroPathogenMeterApp.Models;

namespace AgroPathogenMeterApp
{
    public interface IBtControl
    {
        Task<BtDatabase> TestConn();
        //void Connect();

        String FilePath();
    }
}
