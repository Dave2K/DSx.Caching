using DSx.Caching.Abstractions.Models;

/// <summary>
/// Interfaccia base per tutti i provider di cache
/// </summary>
public interface ICacheProvider : IDisposable, IAsyncDisposable
{
    /// <summary>Verifica l'esistenza di una chiave</summary>
    Task<CacheOperationResult> ExistsAsync(string key, CacheEntryOptions? options = null, CancellationToken ct = default);

    /// <summary>Ottiene un valore dalla cache</summary>
    Task<CacheOperationResult<T>> GetAsync<T>(string key, CacheEntryOptions? options = null, CancellationToken ct = default);

    /// <summary>Imposta un valore nella cache</summary>
    Task<CacheOperationResult> SetAsync<T>(string key, T value, CacheEntryOptions? options = null, CancellationToken ct = default);

    /// <summary>Rimuove una chiave dalla cache</summary>
    Task<CacheOperationResult> RemoveAsync(string key, CancellationToken ct = default);

    /// <summary>Svuota completamente la cache</summary>
    Task<CacheOperationResult> ClearAllAsync(CancellationToken ct = default);
}