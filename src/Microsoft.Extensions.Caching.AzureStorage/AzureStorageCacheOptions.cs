namespace Microsoft.Extensions.Caching.AzureStorage
{
    public class AzureStorageCacheOptions
    {
        public const string AzureStorageCache = "AzureStorageCache";

        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string PartitionKey { get; set; }
    }
}
