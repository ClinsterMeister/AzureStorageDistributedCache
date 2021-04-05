using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Caching.AzureStorage;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureStorageCacheServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Azure Storage distributed caching services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configuration">An <see cref="Action{AzureStorageCacheOptions}"/> to configure the provided
        /// <see cref="AzureStorageCacheOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddAzureStorageCache(this IServiceCollection services, Action<AzureStorageCacheOptions> configuration)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddOptions();
            services.Configure(configuration);
            services.AddSingleton<IDistributedCache, AzureStorageCache>();

            return services;
        }

        /// <summary>
        /// Adds Azure Storage distributed caching services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configuration"><see cref="IConfiguration"/> from appsettings.json</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddAzureStorageCache(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            services.AddOptions();
            services.Configure<AzureStorageCacheOptions>(configuration.GetSection(AzureStorageCacheOptions.AzureStorageCache));
            services.AddSingleton<IDistributedCache, AzureStorageCache>();

            return services;
        }

        /// <summary>
        /// Adds Azure Storage distributed caching services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddAzureStorageCache(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CACHE_CONNECTION_STRING") 
                ?? throw new NullReferenceException($"Missing Environment Variable 'AZURE_STORAGE_CACHE_CONNECTION_STRING' which is required for {nameof(AzureStorageCache)}");
            string tableName = Environment.GetEnvironmentVariable("AZURE_STORAGE_CACHE_TABLE_NAME")
                ?? throw new NullReferenceException($"Missing Environment Variable 'AZURE_STORAGE_CACHE_TABLE_NAME' which is required for {nameof(AzureStorageCache)}");
            string partitionKey = Environment.GetEnvironmentVariable("AZURE_STORAGE_CACHE_PARTITION_KEY")
                ?? throw new NullReferenceException($"Missing Environment Variable 'AZURE_STORAGE_CACHE_PARTITION_KEY' which is required for {nameof(AzureStorageCache)}");

            services.AddOptions();
            services.Configure<AzureStorageCacheOptions>(configureOptions =>
            {
                configureOptions.ConnectionString = connectionString;
                configureOptions.TableName = tableName;
                configureOptions.PartitionKey = partitionKey;
            });
            services.AddSingleton<IDistributedCache, AzureStorageCache>();

            return services;
        }
    }
}
