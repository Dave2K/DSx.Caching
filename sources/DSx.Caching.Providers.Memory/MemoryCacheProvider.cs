namespace DSx.Caching.Providers.Memory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DSx.Caching.Abstractions.Interfaces;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// In-memory cache provider implementation using Microsoft.Extensions.Caching.Memory
    /// </summary>
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheProvider> _logger;

        public MemoryCacheProvider(
            IMemoryCache cache,
            ILogger<MemoryCacheProvider> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            ValidateKey(key);
            cancellationToken.ThrowIfCancellationRequested();
            
            return Task.FromResult(_cache.TryGetValue(key, out _));
        }

        /// <inheritdoc/>
        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            ValidateKey(key);
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(_cache.Get<T>(key));
        }

        /// <inheritdoc/>
        public Task<bool> SetAsync<T>(
            string key, 
            T value, 
            TimeSpan expiration, 
            CancellationToken cancellationToken = default)
        {
            ValidateKey(key);
            cancellationToken.ThrowIfCancellationRequested();

            _cache.Set(key, value, expiration);
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            ValidateKey(key);
            cancellationToken.ThrowIfCancellationRequested();

            _cache.Remove(key);
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<bool> ClearAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_cache is MemoryCache memCache)
                memCache.Compact(1.0);
            
            return Task.FromResult(true);
        }

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be empty", nameof(key));
        }
    }
}