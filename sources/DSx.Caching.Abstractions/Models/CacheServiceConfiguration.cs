using System;

namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Rappresenta una voce nella cache con metadati
    /// </summary>
    /// <typeparam name="T">Tipo del valore memorizzato</typeparam>
    public class CacheEntry<T>
    {
        /// <summary>
        /// Valore memorizzato nella cache
        /// </summary>
        /// <value>Istanza del tipo generico T</value>
        public T Value { get; set; } = default!;

        /// <summary>
        /// Data/ora di creazione della voce
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Data/ora di scadenza della voce
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }

        /// <summary>
        /// Contatore degli accessi per politiche di cache
        /// </summary>
        public int AccessCount { get; set; }

        /// <summary>
        /// Dimensione approssimativa in byte (per politiche di eviction)
        /// </summary>
        public long Size { get; set; }
    }

    /// <summary>
    /// Enum che definisce le operazioni base sulla cache
    /// </summary>
    public enum CacheOperation
    {
        /// <summary>Operazione di lettura</summary>
        Read,

        /// <summary>Operazione di scrittura</summary>
        Write,

        /// <summary>Operazione di eliminazione</summary>
        Delete,

        /// <summary>Operazione di aggiornamento</summary>
        Update
    }

    /// <summary>
    /// Configurazione globale per il servizio di caching
    /// </summary>
    public class CacheServiceConfiguration
    {
        private string _readStrategy = "CacheAside";
        private string _writeStrategy = "WriteThrough";

        /// <summary>
        /// Durata predefinita per le voci nella cache
        /// </summary>
        /// <value>
        /// Valore <see cref="TimeSpan"/> che rappresenta l'intervallo di tempo.
        /// Esempio: <c>TimeSpan.FromMinutes(30)</c>
        /// </value>
        public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Strategia di lettura dalla cache
        /// </summary>
        /// <value>
        /// Valori supportati:
        /// <list type="bullet">
        /// <item><description><c>CacheAside</c>: Carica i dati su richiesta (default)</description></item>
        /// <item><description><c>ReadThrough</c>: Carica automaticamente dall'origine dati</description></item>
        /// <item><description><c>RefreshAhead</c>: Aggiorna prima della scadenza</description></item>
        /// </list>
        /// </value>
        /// <exception cref="ArgumentNullException">Se si tenta di impostare un valore null</exception>
        public string ReadStrategy
        {
            get => _readStrategy;
            set => _readStrategy = value switch
            {
                "CacheAside" or "ReadThrough" or "RefreshAhead" => value,
                null => throw new ArgumentNullException(nameof(value)),
                _ => throw new ArgumentException("Strategia di lettura non valida")
            };
        }

        /// <summary>
        /// Strategia di scrittura nella cache
        /// </summary>
        /// <value>
        /// Valori supportati:
        /// <list type="bullet">
        /// <item><description><c>WriteThrough</c>: Scrive sia in cache che nell'origine dati (default)</description></item>
        /// <item><description><c>WriteBehind</c>: Scrive prima in cache e poi in batch nell'origine dati</description></item>
        /// <item><description><c>WriteAround</c>: Scrive direttamente nell'origine dati</description></item>
        /// </list>
        /// </value>
        /// <exception cref="ArgumentNullException">Se si tenta di impostare un valore null</exception>
        public string WriteStrategy
        {
            get => _writeStrategy;
            set => _writeStrategy = value switch
            {
                "WriteThrough" or "WriteBehind" or "WriteAround" => value,
                null => throw new ArgumentNullException(nameof(value)),
                _ => throw new ArgumentException("Strategia di scrittura non valida")
            };
        }

        /// <summary>
        /// Intervallo per la pulizia automatica della cache
        /// </summary>
        /// <example>
        /// <code>
        /// var config = new CacheServiceConfiguration {
        ///     CleanupInterval = TimeSpan.FromHours(1)
        /// };
        /// </code>
        /// </example>
        public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// Dimensione massima della cache in megabyte
        /// </summary>
        /// <value>
        /// Valore intero positivo. Valore default: 1024 (1GB)
        /// </value>
        public int MaxMemorySize { get; set; } = 1024;

        /// <summary>
        /// Abilita la compressione dei valori nella cache
        /// </summary>
        /// <remarks>
        /// La compressione utilizza l'algoritmo GZip. 
        /// Utile per valori grandi ma aumenta il carico CPU.
        /// </remarks>
        public bool EnableCompression { get; set; } = true;

        /// <summary>
        /// Abilita il tracciamento delle metriche della cache
        /// </summary>
        /// <value>
        /// Se true, tiene traccia di: hit rate, miss rate, utilizzo memoria
        /// </value>
        public bool EnableMetrics { get; set; } = true;
    }
}