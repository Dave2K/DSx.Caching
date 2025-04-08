using System;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions
{
    /// <summary>
    /// Defines the contract for caching operations.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves an item from the cache by key.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Stores an item in the cache.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="key"/> is null or <paramref name="value"/> is null.
        /// </exception>
        Task SetAsync<T>(string key, T value);

        /// <summary>
        /// Removes an item from the cache by key.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        Task RemoveAsync(string key);
    }
}