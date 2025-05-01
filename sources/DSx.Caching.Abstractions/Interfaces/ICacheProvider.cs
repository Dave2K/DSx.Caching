using DSx.Caching.Abstractions.Events;
using DSx.Caching.Abstractions.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Definisce le operazioni fondamentali per un provider di caching, supportando operazioni asincrone
    /// e notifiche di evento prima e dopo ogni operazione.
    /// </summary>
    public interface ICacheProvider : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Evento sollevato prima dell'esecuzione di qualsiasi operazione sul provider.
        /// </summary>
        event EventHandler<CacheEventArgs>? BeforeOperation;

        /// <summary>
        /// Evento sollevato dopo il completamento di qualsiasi operazione sul provider.
        /// </summary>
        event EventHandler<CacheEventArgs>? AfterOperation;

        /// <summary>
        /// Memorizza un valore nella cache associandolo a una chiave specifica.
        /// </summary>
        /// <typeparam name="T">Tipo del valore da memorizzare.</typeparam>
        /// <param name="key">Chiave univoca di identificazione del valore.</param>
        /// <param name="value">Valore da memorizzare nella cache.</param>
        /// <param name="options">Opzioni configurabili per la memorizzazione (scadenza, priorità, ecc.).</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione.</param>
        /// <returns>Risultato dell'operazione con stato e dettagli eventuali.</returns>
        Task<CacheOperationResult> SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera un valore dalla cache utilizzando la chiave specificata.
        /// </summary>
        /// <typeparam name="T">Tipo del valore da recuperare.</typeparam>
        /// <param name="key">Chiave univoca del valore da recuperare.</param>
        /// <param name="options">Opzioni aggiuntive per il recupero (es. refresh automatico).</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione.</param>
        /// <returns>Risultato contenente il valore recuperato o eventuali errori.</returns>
        Task<CacheOperationResult<T>> GetAsync<T>(
            string key,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rimuove un valore dalla cache utilizzando la chiave specificata.
        /// </summary>
        /// <param name="key">Chiave univoca del valore da rimuovere.</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione.</param>
        /// <returns>Risultato dell'operazione con stato e dettagli eventuali.</returns>
        Task<CacheOperationResult> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene i metadati descrittivi di una voce nella cache.
        /// </summary>
        /// <param name="key">Chiave univoca della voce da analizzare.</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione.</param>
        /// <returns>Risultato contenente i metadati della voce o eventuali errori.</returns>
        Task<CacheOperationResult<CacheEntryDescriptor>> GetDescriptorAsync(
            string key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Svuota completamente il contenuto della cache.
        /// </summary>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione.</param>
        /// <returns>Risultato dell'operazione con stato e dettagli eventuali.</returns>
        Task<CacheOperationResult> ClearAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica l'esistenza di una chiave nella cache.
        /// </summary>
        /// <param name="key">Chiave da verificare.</param>
        /// <param name="cancellationToken">Token per l'annullamento dell'operazione.</param>
        /// <returns>Risultato contenente l'esito della verifica o eventuali errori.</returns>
        Task<CacheOperationResult<bool>> ExistsAsync(
            string key,
            CancellationToken cancellationToken = default);
    }
}
