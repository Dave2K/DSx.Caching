using System;

namespace DSx.Caching.Abstractions.Attributes
{
    /// <summary>
    /// Attributo per contrassegnare metodi i cui risultati devono essere memorizzati nella cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CacheableAttribute : Attribute
    {
        /// <summary>
        /// Prefisso personalizzato per la generazione della chiave di cache. 
        /// Se non specificato, viene usato il nome del metodo.
        /// </summary>
        public string CacheKeyPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Durata di validità del dato in cache. 
        /// Valore zero indica scadenza determinata dalle policy di cache.
        /// </summary>
        public TimeSpan Expiration { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Abilita l'aggiornamento asincrono della cache dopo la scadenza
        /// </summary>
        public bool EnableBackgroundRefresh { get; set; } = false;

        /// <summary>
        /// Specifica il provider di cache da utilizzare (default: prende quello principale)
        /// </summary>
        public string CacheProvider { get; set; } = "Default";
    }
}
