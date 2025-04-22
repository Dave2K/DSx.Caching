using System;

namespace DSx.Caching.Abstractions.Events
{
    /// <summary>
    /// Definisce il contratto per i sottoscrittori agli eventi della cache.
    /// </summary>
    public interface ICacheEventSubscriber
    {
        /// <summary>
        /// Metodo chiamato prima di un'operazione sulla cache.
        /// </summary>
        /// <param name="sender">Oggetto che ha generato l'evento.</param>
        /// <param name="args">Argomenti dell'evento.</param>
        void OnBeforeOperation(object sender, CacheEventArgs args);

        /// <summary>
        /// Metodo chiamato dopo un'operazione sulla cache.
        /// </summary>
        /// <param name="sender">Oggetto che ha generato l'evento.</param>
        /// <param name="args">Argomenti dell'evento.</param>
        void OnAfterOperation(object sender, CacheEventArgs args);
    }
}