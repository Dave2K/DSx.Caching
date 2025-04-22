using System;

namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Opzioni configurabili per una voce nella cache.
    /// </summary>
    public class CacheEntryOptions
    {
        private TimeSpan? _absoluteExpiration;
        private TimeSpan? _slidingExpiration;

        /// <summary>
        /// Durata massima di validità della voce dalla creazione.
        /// </summary>
        /// <exception cref="ArgumentException">Se il valore è zero o negativo.</exception>
        public TimeSpan? AbsoluteExpiration
        {
            get => _absoluteExpiration;
            set
            {
                if (value.HasValue && value.Value <= TimeSpan.Zero)
                    throw new ArgumentException("La scadenza assoluta deve essere maggiore di zero.", nameof(value));
                _absoluteExpiration = value;
            }
        }

        /// <summary>
        /// Durata di validità dopo l'ultimo accesso alla voce.
        /// </summary>
        /// <exception cref="ArgumentException">Se il valore è zero o negativo.</exception>
        public TimeSpan? SlidingExpiration
        {
            get => _slidingExpiration;
            set
            {
                if (value.HasValue && value.Value <= TimeSpan.Zero)
                    throw new ArgumentException("La scadenza sliding deve essere maggiore di zero.", nameof(value));
                _slidingExpiration = value;
            }
        }

        /// <summary>
        /// Priorità della voce per la gestione della memoria.
        /// </summary>
        public CacheEntryPriority Priority { get; set; } = CacheEntryPriority.Normal;
    }

    /// <summary>
    /// Definisce la priorità di una voce nella cache.
    /// </summary>
    public enum CacheEntryPriority
    {
        /// <summary>Priorità bassa (più probabile rimozione).</summary>
        Low,
        /// <summary>Priorità standard.</summary>
        Normal,
        /// <summary>Priorità alta (meno probabile rimozione).</summary>
        High
    }
}