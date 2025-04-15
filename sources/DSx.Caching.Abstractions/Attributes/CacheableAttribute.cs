using System;

namespace DSx.Caching.Abstractions.Attributes
{
    /// <summary>
    /// Attributo per marcare i metodi i cui risultati devono essere memorizzati in cache
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheableAttribute : Attribute
    {
        /// <summary>
        /// Prefisso opzionale per la chiave di cache
        /// </summary>
        public string CacheKeyPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Durata di validità della cache
        /// </summary>
        public TimeSpan Expiration { get; set; }
    }
}