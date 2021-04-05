using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.AzureStorage
{
    public class AzureStorageCache : IDistributedCache
    {
        private readonly IAzureStorageProvider _provider;

        public AzureStorageCache(IOptions<AzureStorageCacheOptions> options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(options.Value.ConnectionString);
            CloudTableClient cloutTableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable cloudTable = cloutTableClient.GetTableReference(options.Value.TableName);

            if (cloudTable.Exists() == false)
                cloudTable.Create();

            _provider = new AzureStorageProvider(cloudTable, options.Value.PartitionKey);
        }


        public byte[] Get(string key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            TableCacheEntity cacheEntity = _provider.RetrieveItem(key);
            if (cacheEntity is null)
            {
                return null;
            }

            RefreshEntity(cacheEntity);

            return cacheEntity.Data;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            TableCacheEntity cacheEntity = await _provider.RetrieveItemAsync(key, token);
            if (cacheEntity is null)
            {
                return null;
            }

            await RefreshEntityAsync(cacheEntity);

            return cacheEntity?.Data;
        }

        public void Refresh(string key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            TableCacheEntity cacheEntity = _provider.RetrieveItem(key);

            if (cacheEntity != null)
                RefreshEntity(cacheEntity);
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            TableCacheEntity cacheEntity = await _provider.RetrieveItemAsync(key, token);

            if (cacheEntity != null)
                await RefreshEntityAsync(cacheEntity);
        }

        public void Remove(string key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            _provider.RemoveItem(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return _provider.RemoveItemAsync(key, token);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            _provider.InsertOrUpdateItem(key, value, options);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            if (value is null) throw new ArgumentNullException(nameof(value));

            return _provider.InsertOrUpdateItemAsync(key, value, options, token);
        }

        private async Task<TableCacheEntity> RefreshEntityAsync(TableCacheEntity cacheEntity, CancellationToken token = default)
        {
            if (cacheEntity is null)
            {
                return null;
            }

            if (cacheEntity.Data is null || ShouldDelete(cacheEntity))
            {
                await _provider.RemoveItemAsync(cacheEntity, token);
                return null;
            }

            cacheEntity.LastAccessedTime = DateTimeOffset.UtcNow;
            await _provider.InsertOrUpdateItemAsync(cacheEntity, token);
            return cacheEntity;
        }

        private TableCacheEntity RefreshEntity(TableCacheEntity cacheEntity)
        {
            if (cacheEntity is null)
            {
                return null;
            }

            if (cacheEntity.Data is null || ShouldDelete(cacheEntity))
            {
                _provider.RemoveItem(cacheEntity);
                return null;
            }

            cacheEntity.LastAccessedTime = DateTimeOffset.UtcNow;
            _provider.InsertOrUpdateItem(cacheEntity);
            return cacheEntity;
        }

        private bool ShouldDelete(TableCacheEntity cacheEntity)
        {
            DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            if (cacheEntity.AbsoluteExpiration.HasValue && cacheEntity.AbsoluteExpiration.Value <= currentTime)
            {
                return true;
            }

            if (cacheEntity.SlidingExpiration.HasValue && cacheEntity.LastAccessedTime.HasValue &&
                cacheEntity.LastAccessedTime.Value.Add(cacheEntity.SlidingExpiration.Value) < currentTime)
            {
                return true;
            }

            return false;
        }
    }
}
