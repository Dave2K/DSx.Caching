using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Caching
{
    /// <summary>
    /// Provides protection against cache stampede effects by using key-specific semaphores
    /// </summary>
    /// <remarks>
    /// This implementation uses in-memory locks and is suitable for single-node applications.
    /// For distributed scenarios, use a distributed lock mechanism instead.
    /// </remarks>
    public class StampedeProtector
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new();

        /// <summary>
        /// Safely executes a value factory function with stampede protection
        /// </summary>
        /// <typeparam name="T">Type of the cached value</typeparam>
        /// <param name="key">The cache key to protect</param>
        /// <param name="valueFactory">Async factory function to generate the value</param>
        /// <returns>The generated or cached value</returns>
        /// <exception cref="ArgumentNullException">Thrown if key or valueFactory are null</exception>
        public async Task<T> ExecuteWithLockAsync<T>(string key, Func<Task<T>> valueFactory)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            var keyLock = _keyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            await keyLock.WaitAsync();
            try
            {
                return await valueFactory();
            }
            finally
            {
                keyLock.Release();
                _keyLocks.TryRemove(key, out _);
            }
        }
    }
}