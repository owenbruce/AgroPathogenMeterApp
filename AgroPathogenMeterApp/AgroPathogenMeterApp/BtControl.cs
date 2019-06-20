using AgroPathogenMeterApp.Models;

namespace AgroPathogenMeterApp
{
    public interface BtControl
    {
        void connect();

        void connect(ScanDatabase _database);

        string FilePath();
    }
}