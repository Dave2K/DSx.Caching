using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Caching
{
    /// <summary>
    /// Previene accessi concorrenti alla stessa risorsa tramite lock per chiave
    /// </summary>
    public sealed class StampedeProtector
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new();

        /// <summary>
        /// Numero di lock attualmente attivi (solo per testing)
        /// </summary>
        public int ActiveLocksCount => _keyLocks.Count;

        /// <summary>
        /// Esegue un'operazione con lock distribuito per chiave
        /// </summary>
        public async Task<T> ExecuteWithLockAsync<T>(string key, Func<Task<T>> valueFactory)
        {
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