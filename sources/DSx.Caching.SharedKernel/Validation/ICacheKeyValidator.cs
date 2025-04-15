namespace DSx.Caching.SharedKernel.Validation
{
    /// <summary>
    /// Interfaccia per la validazione e normalizzazione delle chiavi di cache
    /// </summary>
    public interface ICacheKeyValidator
    {
        /// <summary>
        /// Verifica la validità del formato di una chiave
        /// </summary>
        /// <param name="key">Chiave da validare</param>
        void Validate(string key);

        /// <summary>
        /// Trasforma una chiave in un formato standardizzato
        /// </summary>
        /// <param name="key">Chiave da normalizzare</param>
        /// <returns>Chiave nel formato accettato dal sistema</returns>
        string NormalizeKey(string key);
    }
}