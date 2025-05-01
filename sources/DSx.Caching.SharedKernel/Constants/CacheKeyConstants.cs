namespace DSx.Caching.SharedKernel.Constants
{
    /// <summary>
    /// Contiene costanti per la gestione delle chiavi della cache
    /// </summary>
    public static class CacheKeyConstants
    {
        /// <summary>
        /// Pattern regex per la validazione delle chiavi. Ammette lettere, numeri e trattini (1-128 caratteri)
        /// </summary>
        public const string KeyValidationPattern = @"^[a-zA-Z0-9\-]{1,128}$";

        /// <summary>
        /// Pattern regex per normalizzare le chiavi. Sostituisce caratteri non alfanumerici/trattini con "-"
        /// </summary>
        public const string KeyNormalizationPattern = @"[^a-zA-Z0-9\-]";

        /// <summary>
        /// Lunghezza massima consentita per una chiave dopo la normalizzazione
        /// </summary>
        public const int MaxKeyLength = 128;
    }
}
