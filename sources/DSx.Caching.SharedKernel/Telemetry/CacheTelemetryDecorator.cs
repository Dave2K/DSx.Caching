using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DSx.Caching.Abstractions;
using DSx.Caching.Abstractions.Events;
using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Telemetry;
using Microsoft.Extensions.Logging;

namespace DSx.Caching.SharedKernel.Telemetry
{
    /// <summary>
    /// Decoratore per aggiungere telemetria alle operazioni della cache
    /// </summary>
    public class CacheTelemetryDecorator : ICacheProvider, IAsyncDisposable, IDisposable
    {
        private readonly ICacheProvider _inner;
        private readonly ICacheTelemetry _telemetry;
        private readonly ILogger<CacheTelemetryDecorator> _logger;
        private bool _disposed;

        /// <summary>
        /// Costruttore principale
        /// </summary>
        public CacheTelemetryDecorator(
            ICacheProvider inner,
            ICacheTelemetry telemetry,
            ILogger<CacheTelemetryDecorator> logger)
        {
            _inner = inner;
            _telemetry = telemetry;
            _logger = logger;

            _inner.BeforeOperation += OnBeforeOperation;
            _inner.AfterOperation += OnAfterOperation;
        }

        /// <summary>
        /// Evento sollevato prima di un'operazione sulla cache
        /// </summary>
        public event EventHandler<CacheEventArgs>? BeforeOperation;

        /// <summary>
        /// Evento sollevato dopo un'operazione sulla cache
        /// </summary>
        public event EventHandler<CacheEventArgs>? AfterOperation;

        /// <summary>
        /// Esegue un'operazione di recupero dalla cache con tracciatura telemetria
        /// </summary>
        /// <typeparam name="T">Tipo del dato da recuperare</typeparam>
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
        /// Esegue un'operazione di salvataggio nella cache con tracciatura telemetria
        /// </summary>
        /// <typeparam name="T">Tipo del dato da salvare</typeparam>
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
        /// Esegue un'operazione di rimozione dalla cache con tracciatura telemetria
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
        /// Esegue la pulizia completa della cache con tracciatura telemetria
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
        /// Verifica l'esistenza di una chiave nella cache con tracciatura telemetria
        /// </summary>
        public async Task<CacheOperationResult<bool>> ExistsAsync(
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
        /// Recupera i metadati di una voce della cache con tracciatura telemetria
        /// </summary>
        public async Task<CacheOperationResult<CacheEntryDescriptor>> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            try
            {
                var result = await _inner.GetDescriptorAsync(key, cancellationToken);
                TrackDependency("GetDescriptor", key, result.Status == CacheOperationStatus.Success, startTime);
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
        /// Implementazione dello smaltimento sincrono delle risorse
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementazione dello smaltimento asincrono delle risorse
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Logica centrale di smaltimento sincrono
        /// </summary>
        /// <param name="disposing">Indica se è in corso uno smaltimento esplicito</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _inner.BeforeOperation -= OnBeforeOperation;
                _inner.AfterOperation -= OnAfterOperation;

                if (_inner is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Logica centrale di smaltimento asincrono
        /// </summary>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_disposed) return;

            _inner.BeforeOperation -= OnBeforeOperation;
            _inner.AfterOperation -= OnAfterOperation;

            if (_inner is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else if (_inner is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _disposed = true;
        }

        private void OnBeforeOperation(object? sender, CacheEventArgs args)
        {
            BeforeOperation?.Invoke(this, args);
        }

        private void OnAfterOperation(object? sender, CacheEventArgs args)
        {
            AfterOperation?.Invoke(this, args);
        }

        private void TrackDependency(
            string operation,
            string key,
            bool success,
            DateTimeOffset startTime)
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
            _telemetry.TrackException(ex, new Dictionary<string, object>
            {
                ["Operation"] = operation,
                ["Key"] = key
            });
        }
    }
}
