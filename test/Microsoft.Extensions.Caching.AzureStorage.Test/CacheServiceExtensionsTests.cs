using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Microsoft.Extensions.Caching.AzureStorage.Test
{
    [TestClass]
    public class CacheServiceExtensionsTests
    {
        [TestMethod]
        public void AddAzureStorageCache_RegistersDistributedCacheAsSingleton()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();

            // Act
            services.AddAzureStorageCache(options => { });

            // Assert
            var distributedCache = services.FirstOrDefault(desc => desc.ServiceType == typeof(IDistributedCache));

            Assert.IsNotNull(distributedCache);
            Assert.AreEqual(ServiceLifetime.Singleton, distributedCache.Lifetime);
        }
    }
}
