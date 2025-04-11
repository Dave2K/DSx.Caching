using DSx.Caching.SharedKernel.Enums;
using Microsoft.Extensions.Logging;
using System;

namespace DSx.Caching.Providers.Redis
{
    /// <summary>
    /// Eccezione specifica per errori nel provider Redis
    /// </summary>
    public class RedisCacheException : Exception
    {
        /// <summary>
        /// Dettagli tecnici dell'errore (NUOVA IMPLEMENTAZIONE)
        /// </summary>
        public string TechnicalDetails { get; }

        /// <summary>
        /// Crea una nuova istanza dell'eccezione
        /// </summary>
        public RedisCacheException(
            ILogger<RedisCacheProvider> logger,
            string message,
            string technicalDetails,
            Exception? inner = null)
            : base(message, inner)
        {
            TechnicalDetails = technicalDetails;

            logger.LogError(
                "Errore Redis: {Message}. Dettagli: {Details}",
                message,
                TechnicalDetails
            );
        }
    }
}