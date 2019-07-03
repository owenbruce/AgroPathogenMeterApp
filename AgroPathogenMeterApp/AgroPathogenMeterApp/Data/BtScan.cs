using AgroPathogenMeterApp.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Data
{
    public class Scanner2
    {
        private readonly SQLiteAsyncConnection _database;

        public Scanner2(string dbPath)   //The current running database is saved as a table in the sqlite database
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<ScanDatabase>().Wait();
        }

        public Task<List<BtDatabase>> GetScanDatabasesAsync()   //Retrieves all of the "databases"
        {
            return _database.Table<BtDatabase>().ToListAsync();
        }

        public Task<BtDatabase> GetScanAsync(int id)   //Gets a ceartain scan from the database
        {
            return _database.Table<BtDatabase>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveScanAsync(BtDatabase scan)   //Saves the current scan into the database
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

        public Task<int> DeleteScanAsync(BtDatabase scan)   //Deletes the current scan from the database
        {
            return _database.DeleteAsync(scan);
        }
    }
}