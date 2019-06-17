using AgroPathogenMeterApp.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Data
{
    public class Scanner
    {
        private readonly SQLiteAsyncConnection _database;

        public Scanner(string dbPath)   //The current running database is saved as a table in the sqlite database
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<ScanDatabase>().Wait();
        }

        public Task<int> DeleteScanAsync(ScanDatabase scan)   //Deletes the current scan from the database
        {
            return _database.DeleteAsync(scan);
        }

        public Task<ScanDatabase> GetScanAsync(int id)   //Gets a ceartain scan from the database
        {
            return _database.Table<ScanDatabase>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<List<ScanDatabase>> GetScanDatabasesAsync()   //Retrieves all of the "databases"
        {
            return _database.Table<ScanDatabase>().ToListAsync();
        }
        public Task<int> SaveScanAsync(ScanDatabase scan)   //Saves the current scan into the database
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
    }
}