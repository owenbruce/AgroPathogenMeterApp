using AgroPathogenMeterApp.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Data
{
    public class Scanner
    {
        readonly SQLiteAsyncConnection _database;

        public Scanner(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<ScanDatabase>().Wait();
        }

        public Task<List<ScanDatabase>> GetScanDatabasesAsync()
        {
            return _database.Table<ScanDatabase>().ToListAsync();
        }

        public Task<ScanDatabase> GetNoteAsync(int id)
        {
            return _database.Table<ScanDatabase>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }
        public Task<int> SaveNoteAsync(ScanDatabase scan)
        {
            if (scan.ID != 0)
            {
                return _database.UpdateAsync(scan);
            }
            else
            {
                return _database.InsertAsync(scan);
            }
        }
        public Task<int> DeleteNoteAsync(ScanDatabase scan)
        {
            return _database.DeleteAsync(scan);
        }
    }
}
