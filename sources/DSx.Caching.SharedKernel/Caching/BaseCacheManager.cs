using DSx.Caching.Abstractions;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Caching
{
    /// <summary>
    /// Classe base per la gestione centralizzata delle operazioni di cache
    /// </summary>
    public abstract class BaseCacheManager
    {
        private readonly ILogger _logger;
        private readonly ICacheProvider _cacheProvider;

        /// <summary>
        /// Costruttore principale
        /// </summary>
        protected BaseCacheManager(
            ILogger logger,
            ICacheProvider cacheProvider)
        {
            _logger = logger;
            _cacheProvider = cacheProvider;
        }

        /// <summary>
        /// Esegue un'operazione generica con gestione centralizzata degli errori
        /// </summary>
        /// <typeparam name="T">Tipo del dato restituito</typeparam>
        protected virtual async Task<CacheOperationResult<T>> ExecuteOperationAsync<T>(
            string key,
            Func<Task<CacheOperationResult<T>>> operation)
        {
            try
            {
                var result = await operation();
                return new CacheOperationResult<T>(
                    result.Value,
                    result.Status,
                    result.Details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operazione fallita per la chiave: {Key}", key);
                return new CacheOperationResult<T>(
                    default,
                    CacheOperationStatus.ValidationError,
                    ex.Message);
            }
        }
    }
}
