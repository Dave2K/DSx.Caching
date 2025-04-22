using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Caching
{
    /// <summary>
    /// Previene l'accesso concorrente a risorse critiche
    /// </summary>
    public sealed class StampedeProtector
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new();

        /// <summary>
        /// Esegue un'operazione con lock distribuito
        /// </summary>
        /// <typeparam name="T">Tipo del risultato</typeparam>
        /// <param name="key">Chiave su cui applicare il lock</param>
        /// <param name="valueFactory">Funzione da eseguire protetta</param>
        /// <returns>Risultato dell'operazione</returns>
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

        /// <summary>
        /// Numero di lock attualmente attivi
        /// </summary>
        public int ActiveLocksCount => _keyLocks.Count;
    }
}