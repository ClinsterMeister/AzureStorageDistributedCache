using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Caching.Distributed;
using System;

namespace Microsoft.Extensions.Caching.AzureStorage
{
    public sealed class TableCacheEntity : TableEntity
    {
        [Obsolete]
        public TableCacheEntity() { }

        public TableCacheEntity(string partitionKey, string rowKey, byte[] data = null, DistributedCacheEntryOptions options = null)
            : base(partitionKey, rowKey)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            if (options?.AbsoluteExpirationRelativeToNow != null)
            {
                AbsoluteExpiration = utcNow.Add(options.AbsoluteExpirationRelativeToNow.Value);
            }
            else if (options?.AbsoluteExpiration != null)
            {
                if (options.AbsoluteExpiration <= utcNow)
                {
                    throw new ArgumentOutOfRangeException(nameof(options.AbsoluteExpiration), options.AbsoluteExpiration.Value, "Absolute expiration value must be in the future.");
                }

                AbsoluteExpiration = options.AbsoluteExpiration;
            }

            Data = data;
            SlidingExpiration = options?.SlidingExpiration;
            LastAccessedTime = utcNow;
        }

        public byte[] Data { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public DateTimeOffset? LastAccessedTime { get; set; }
    }
}
