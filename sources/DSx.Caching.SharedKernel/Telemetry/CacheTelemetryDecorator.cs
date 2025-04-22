using DSx.Caching.Abstractions;
using DSx.Caching.Abstractions.Events;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Telemetry;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Telemetry
{
    /// <summary>
    /// Decoratore per il tracciamento telemetrico delle operazioni di cache
    /// </summary>
    public class CacheTelemetryDecorator : ICacheProvider, IDisposable
    {
        private readonly ICacheProvider _inner;
        private readonly ICacheTelemetry _telemetry;
        private readonly ILogger<CacheTelemetryDecorator> _logger;

        /// <summary>
        /// Inizializza una nuova istanza del decoratore
        /// </summary>
        public CacheTelemetryDecorator(
            ICacheProvider inner,
            ICacheTelemetry telemetry,
            ILogger<CacheTelemetryDecorator> logger)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Evento sollevato prima di un'operazione sulla cache
        /// </summary>
        public event EventHandler<CacheEventArgs>? BeforeOperation
        {
            add => _inner.BeforeOperation += value;
            remove => _inner.BeforeOperation -= value;
        }

        /// <summary>
        /// Evento sollevato dopo un'operazione sulla cache
        /// </summary>
        public event EventHandler<CacheEventArgs>? AfterOperation
        {
            add => _inner.AfterOperation += value;
            remove => _inner.AfterOperation -= value;
        }

        /// <summary>
        /// Evento sollevato per operazioni differite
        /// </summary>
        public event EventHandler<OperationDeferredEventArgs>? OperationDeferred
        {
            add => _inner.OperationDeferred += value;
            remove => _inner.OperationDeferred -= value;
        }

        /// <summary>
        /// Recupera un valore dalla cache
        /// </summary>
        public async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            try
            {
                var result = await _inner.GetAsync<T>(key, options, cancellationToken);
                TrackDependency("Get", key, result.Status == CacheOperationStatus.Success, startTime);
                return result;
            }
            catch (Exception ex)
            {
                LogError("GET", key, ex);
                TrackDependency("Get", key, false, startTime);
                throw;
            }
        }

        /// <summary>
        /// Memorizza un valore nella cache
        /// </summary>
        public async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            try
            {
                var result = await _inner.SetAsync(key, value, options, cancellationToken);
                TrackDependency("Set", key, result.Status == CacheOperationStatus.Success, startTime);
                return result;
            }
            catch (Exception ex)
            {
                LogError("SET", key, ex);
                TrackDependency("Set", key, false, startTime);
                throw;
            }
        }

        /// <summary>
        /// Verifica l'esistenza di una chiave
        /// </summary>
        public async Task<CacheOperationResult> ExistsAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            try
            {
                var result = await _inner.ExistsAsync(key, cancellationToken);
                TrackDependency("Exists", key, result.Status == CacheOperationStatus.Success, startTime);
                return result;
            }
            catch (Exception ex)
            {
                LogError("EXISTS", key, ex);
                TrackDependency("Exists", key, false, startTime);
                throw;
            }
        }

        /// <summary>
        /// Rimuove una voce dalla cache
        /// </summary>
        public async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            try
            {
                var result = await _inner.RemoveAsync(key, cancellationToken);
                TrackDependency("Remove", key, result.Status == CacheOperationStatus.Success, startTime);
                return result;
            }
            catch (Exception ex)
            {
                LogError("REMOVE", key, ex);
                TrackDependency("Remove", key, false, startTime);
                throw;
            }
        }

        /// <summary>
        /// Svuota completamente la cache
        /// </summary>
        public async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            try
            {
                var result = await _inner.ClearAllAsync(cancellationToken);
                TrackDependency("ClearAll", "ALL_KEYS", result.Status == CacheOperationStatus.Success, startTime);
                return result;
            }
            catch (Exception ex)
            {
                LogError("CLEAR_ALL", "ALL_KEYS", ex);
                TrackDependency("ClearAll", "ALL_KEYS", false, startTime);
                throw;
            }
        }

        /// <summary>
        /// Ottiene i metadati di una voce
        /// </summary>
        public async Task<CacheEntryDescriptor?> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            try
            {
                var result = await _inner.GetDescriptorAsync(key, cancellationToken);
                TrackDependency("GetDescriptor", key, result != null, startTime);
                return result;
            }
            catch (Exception ex)
            {
                LogError("GET_DESCRIPTOR", key, ex);
                TrackDependency("GetDescriptor", key, false, startTime);
                throw;
            }
        }

        /// <summary>
        /// Rilascia le risorse gestite
        /// </summary>
        public void Dispose()
        {
            _inner.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Rilascia le risorse gestite in modo asincrono
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await _inner.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        private void TrackDependency(string operation, string key, bool success, DateTimeOffset startTime)
        {
            _telemetry.TrackDependency(
                "Cache",
                operation,
                key,
                success,
                startTime,
                DateTimeOffset.UtcNow - startTime);
        }

        private void LogError(string operation, string key, Exception ex)
        {
            _logger.LogError(ex, "Errore durante {Operation} per {Key}", operation, key);
        }
    }
}