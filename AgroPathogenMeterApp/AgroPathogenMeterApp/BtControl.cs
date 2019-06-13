using System;
using System.Collections.Generic;
using System.Text;
using AgroPathogenMeterApp.Models;

namespace AgroPathogenMeterApp
{
    public interface BtControl
    {
        void connect();
        void connect(ScanDatabase _database);
    }
}
