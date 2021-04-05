using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.AzureStorage
{
    internal class AzureStorageProvider : IAzureStorageProvider
    {
        private readonly string _partitionKey;
        private readonly CloudTable _cloudTable;

        public AzureStorageProvider(CloudTable cloudTable, string partitionKey)
        {
            _cloudTable = cloudTable ?? throw new ArgumentNullException(nameof(cloudTable));
            _partitionKey = partitionKey;
        }

        public void InsertOrUpdateItem(string key, byte[] data, DistributedCacheEntryOptions options)
        {
            TableCacheEntity cacheEntity = new TableCacheEntity(_partitionKey, key, data, options);
            TableOperation operation = TableOperation.InsertOrReplace(cacheEntity);

            _cloudTable.Execute(operation);
        }

        public void InsertOrUpdateItem(TableCacheEntity entity)
        {
            TableOperation operation = TableOperation.InsertOrReplace(entity);

            _cloudTable.Execute(operation);
        }

        public Task InsertOrUpdateItemAsync(string key, byte[] data, DistributedCacheEntryOptions options, CancellationToken token)
        {
            TableCacheEntity cacheEntity = new TableCacheEntity(_partitionKey, key, data, options);
            TableOperation operation = TableOperation.InsertOrReplace(cacheEntity);

            return _cloudTable.ExecuteAsync(operation, token);
        }

        public Task InsertOrUpdateItemAsync(TableCacheEntity entity, CancellationToken token)
        {
            TableOperation operation = TableOperation.InsertOrReplace(entity);
            return _cloudTable.ExecuteAsync(operation, token);
        }

        public void RemoveItem(string key)
        {
            TableCacheEntity cacheEntity = RetrieveItem(key);
            if (cacheEntity != null)
            {
                RemoveItem(cacheEntity);
            }
        }

        public void RemoveItem(TableCacheEntity entity)
        {
            TableOperation operation = TableOperation.Delete(entity);
            _cloudTable.Execute(operation);
        }

        public async Task RemoveItemAsync(string key, CancellationToken token)
        {
            TableCacheEntity cacheEntity = await RetrieveItemAsync(key, token);
            if (cacheEntity != null)
            {
                await RemoveItemAsync(cacheEntity, token);
            }
        }

        public Task RemoveItemAsync(TableCacheEntity entity, CancellationToken token)
        {
            TableOperation operation = TableOperation.Delete(entity);

            return _cloudTable.ExecuteAsync(operation);
        }

        public TableCacheEntity RetrieveItem(string key)
        {
            TableOperation operation = TableOperation.Retrieve<TableCacheEntity>(_partitionKey, key);
            TableResult result = _cloudTable.Execute(operation);

            return result?.Result as TableCacheEntity ?? null;
        }

        public async Task<TableCacheEntity> RetrieveItemAsync(string key, CancellationToken token)
        {
            TableOperation operation = TableOperation.Retrieve<TableCacheEntity>(_partitionKey, key);
            TableResult result = await _cloudTable.ExecuteAsync(operation, token);

            return result?.Result as TableCacheEntity ?? null;
        }
    }
}
