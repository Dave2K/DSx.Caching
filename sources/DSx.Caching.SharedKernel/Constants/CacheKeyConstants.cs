namespace DSx.Caching.SharedKernel.Constants
{
    /// <summary>
    /// Costanti condivise per la gestione delle chiavi di cache
    /// </summary>
    public static class CacheKeyConstants
    {
        /// <summary>
        /// Pattern regex per la validazione delle chiavi
        /// </summary>
        public const string KeyValidationPattern = @"^[\w\-]{1,128}$";

        /// <summary>
        /// Pattern regex per la normalizzazione delle chiavi
        /// </summary>
        public const string KeyNormalizationPattern = @"[^\w\-]";

        /// <summary>
        /// Lunghezza massima consentita per le chiavi
        /// </summary>
        public const int MaxKeyLength = 128;
    }
}