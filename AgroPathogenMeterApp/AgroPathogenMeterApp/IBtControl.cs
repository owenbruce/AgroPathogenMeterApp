using System;
using System.Collections.Generic;
using System.Text;

namespace AgroPathogenMeterApp
{
    public interface IBtControl
    {
        void TestConn();
        //void Connect();

        String FilePath();
    }
}
