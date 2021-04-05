using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.AzureStorage
{
    internal interface IAzureStorageProvider
    {
        void InsertOrUpdateItem(string key, byte[] data, DistributedCacheEntryOptions options);
        void InsertOrUpdateItem(TableCacheEntity entity);
        Task InsertOrUpdateItemAsync(string key, byte[] data, DistributedCacheEntryOptions options, CancellationToken token);
        Task InsertOrUpdateItemAsync(TableCacheEntity entity, CancellationToken token);
        void RemoveItem(string key);
        void RemoveItem(TableCacheEntity entity);
        Task RemoveItemAsync(string key, CancellationToken token);
        Task RemoveItemAsync(TableCacheEntity entity, CancellationToken token);
        TableCacheEntity RetrieveItem(string key);
        Task<TableCacheEntity> RetrieveItemAsync(string key, CancellationToken token);
    }
}
