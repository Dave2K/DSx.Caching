using DSx.Caching.Abstractions.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Defines the interface for a cache provider with asynchronous operations.
    /// </summary>
    public interface ICacheProvider : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Occurs before a cache operation is performed.
        /// </summary>
        event EventHandler<CacheEventArgs> BeforeOperation;

        /// <summary>
        /// Occurs after a cache operation is performed.
        /// </summary>
        event EventHandler<CacheEventArgs> AfterOperation;

        /// <summary>
        /// Retrieves a value from the cache with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key identifying the entry.</param>
        /// <param name="options">Optional cache entry settings.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A result object containing the operation status and value.</returns>
        Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Stores a value in the cache with the specified key and options.
        /// </summary>
        /// <typeparam name="T">The type of the value to store.</typeparam>
        /// <param name="key">The key identifying the entry.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="options">Optional cache entry settings.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A result object indicating the operation status.</returns>
        Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a cache entry with the specified key.
        /// </summary>
        /// <param name="key">The key identifying the entry.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A result object indicating the operation status.</returns>
        Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Clears all entries from the cache.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A result object indicating the operation status.</returns>
        Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Provides data for cache operation events.
    /// </summary>
    public sealed class CacheEventArgs(string key, CacheOperationType operationType) : EventArgs
    {
        /// <summary>
        /// Gets the cache key associated with the event.
        /// </summary>
        public string Key { get; } = key;

        /// <summary>
        /// Gets the type of cache operation.
        /// </summary>
        public CacheOperationType OperationType { get; } = operationType;
    }

    /// <summary>
    /// Specifies the type of cache operation.
    /// </summary>
    public enum CacheOperationType
    {
        /// <summary>
        /// A read operation.
        /// </summary>
        Get,

        /// <summary>
        /// A write operation.
        /// </summary>
        Set,

        /// <summary>
        /// A removal operation.
        /// </summary>
        Remove,

        /// <summary>
        /// A cache clearance operation.
        /// </summary>
        ClearAll
    }
}