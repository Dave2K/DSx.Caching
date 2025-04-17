using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.Abstractions.Telemetry;
using System.Diagnostics;
using System.Threading;

namespace DSx.Caching.SharedKernel.Telemetry
{
    /// <inheritdoc cref="ICacheProvider"/>
    public class CacheTelemetryDecorator(ICacheProvider inner, ICacheTelemetry telemetry)
        : ICacheProvider, IDisposable, IAsyncDisposable
    {
        private readonly ICacheProvider _inner = inner;
        private readonly ICacheTelemetry _telemetry = telemetry;

        /// <inheritdoc/>
        public event EventHandler<CacheEventArgs>? BeforeOperation
        {
            add => _inner.BeforeOperation += value;
            remove => _inner.BeforeOperation -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<CacheEventArgs>? AfterOperation
        {
            add => _inner.AfterOperation += value;
            remove => _inner.AfterOperation -= value;
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                _telemetry.TrackDependency("Cache", "Get", true);
                return await _inner.GetAsync<T>(key, options, cancellationToken);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex, new Dictionary<string, object> { { "Key", key } });
                throw;
            }
            finally
            {
                sw.Stop();
                _telemetry.TrackRequest("Cache.Get", sw.Elapsed, true);
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                _telemetry.TrackDependency("Cache", "Set", true);
                return await _inner.SetAsync(key, value, options, cancellationToken);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex, new Dictionary<string, object> { { "Key", key } });
                throw;
            }
            finally
            {
                sw.Stop();
                _telemetry.TrackRequest("Cache.Set", sw.Elapsed, true);
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                _telemetry.TrackDependency("Cache", "Remove", true);
                return await _inner.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex, new Dictionary<string, object> { { "Key", key } });
                throw;
            }
            finally
            {
                sw.Stop();
                _telemetry.TrackRequest("Cache.Remove", sw.Elapsed, true);
            }
        }

        /// <inheritdoc/>
        public async Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                _telemetry.TrackDependency("Cache", "ClearAll", true);
                return await _inner.ClearAllAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex, new Dictionary<string, object>());
                throw;
            }
            finally
            {
                sw.Stop();
                _telemetry.TrackRequest("Cache.ClearAll", sw.Elapsed, true);
            }
        }

        /// <inheritdoc/>
        public void Dispose() => (_inner as IDisposable)?.Dispose();

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            if (_inner is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else
                (_inner as IDisposable)?.Dispose();
        }
    }
}