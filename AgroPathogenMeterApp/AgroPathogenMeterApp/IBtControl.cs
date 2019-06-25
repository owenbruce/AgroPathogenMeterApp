using AgroPathogenMeterApp.Models;

namespace AgroPathogenMeterApp
{
    public interface IBtControl
    {
        void connect();

        void connect(ScanDatabase _database);

        string FilePath();
    }
}