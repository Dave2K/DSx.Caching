using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DSx.Caching.Core
{
    /// <summary>
    /// Provides base functionality for cache management operations
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="cacheProvider">The cache provider instance</param>
    public abstract class BaseCacheManager(ILogger logger, ICacheProvider cacheProvider)
    {
        /// <summary>
        /// The logger instance used for logging cache operations
        /// </summary>
        protected readonly ILogger Logger = logger;

        /// <summary>
        /// The cache provider instance used for performing cache operations
        /// </summary>
        protected readonly ICacheProvider CacheProvider = cacheProvider;

        /// <summary>
        /// Generic retrieval operation with error handling
        /// </summary>
        /// <typeparam name="T">Type of cached value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="operation">Cache operation to execute</param>
        /// <returns>Cache operation result</returns>
        protected virtual async Task<CacheOperationResult<T>> GetInternalAsync<T>(
            string key,
            Func<Task<CacheOperationResult<T>>> operation)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Operation failed for key: {Key}", key);
                return new CacheOperationResult<T>
                {
                    Status = CacheOperationStatus.ValidationError,
                    Details = ex.Message
                };
            }
        }
    }
}