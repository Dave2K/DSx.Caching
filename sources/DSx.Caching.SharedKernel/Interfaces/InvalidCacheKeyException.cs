namespace DSx.Caching.SharedKernel.Interfaces
{
    /// <summary>
    /// Eccezione sollevata per chiavi di cache non valide
    /// </summary>
    public class InvalidCacheKeyException : Exception
    {
        /// <summary>
        /// Codice errore univoco
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Valore che ha causato l'errore
        /// </summary>
        public string InvalidValue { get; }

        /// <summary>
        /// Inizializza una nuova istanza dell'eccezione
        /// </summary>
        public InvalidCacheKeyException(
            string message,
            string errorCode,
            string invalidValue
        ) : base(message)
        {
            ErrorCode = errorCode;
            InvalidValue = invalidValue;
        }
    }
}
