using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.AzureStorage
{
    internal static class AzureStorageCacheUtility
    {
		/// <summary>
		/// Wrapper for <see cref="Task.RunSynchronously"/> to safely call Asynchronous code and return result.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task"></param>
		/// <returns></returns>
		internal static T RunAndGetValueSynchronously<T>(this Task<T> task)
        {
            task.RunSynchronously();
            return task.Result;
        }
	}
}
