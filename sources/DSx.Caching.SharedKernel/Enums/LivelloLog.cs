namespace DSx.Caching.SharedKernel.Enums
{
    /// <summary>
    /// Definisce i livelli di gravità per le operazioni di logging
    /// </summary>
    public enum LivelloLog
    {
        /// <summary>
        /// Messaggi diagnostici per il debug
        /// </summary>
        Debug,

        /// <summary>
        /// Informazioni generiche sul funzionamento
        /// </summary>
        Info,

        /// <summary>
        /// Avvisi per situazioni non critiche
        /// </summary>
        Warning,

        /// <summary>
        /// Errori funzionali che richiedono attenzione
        /// </summary>
        Error,

        /// <summary>
        /// Errori critici che bloccano il sistema
        /// </summary>
        Critical
    }
}