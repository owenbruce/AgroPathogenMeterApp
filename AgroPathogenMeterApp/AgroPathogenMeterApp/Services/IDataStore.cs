using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Services
{
    public interface IDataStore<T>   //More example data storage w/ azure
    {
        Task<bool> AddItemAsync(T item);

        Task<bool> UpdateItemAsync(T item);

        Task<bool> DeleteItemAsync(string id);

        Task<T> GetItemAsync(string id);

        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}