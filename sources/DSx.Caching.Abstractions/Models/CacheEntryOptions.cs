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
        /// <exception cref="ArgumentException">Se il valore è negativo.</exception>
        public TimeSpan? AbsoluteExpiration
        {
            get => _absoluteExpiration;
            set
            {
                if (value?.Ticks < 0)
                    throw new ArgumentException("Il valore non può essere negativo.", nameof(value));
                _absoluteExpiration = value;
            }
        }

        /// <summary>
        /// Durata di validità dopo l'ultimo accesso alla voce.
        /// </summary>
        /// <exception cref="ArgumentException">Se il valore è negativo.</exception>
        public TimeSpan? SlidingExpiration
        {
            get => _slidingExpiration;
            set
            {
                if (value?.Ticks < 0)
                    throw new ArgumentException("Il valore non può essere negativo.", nameof(value));
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
        /// <summary> Priorità standard. </summary>
        Normal,
        /// <summary> Priorità alta (meno probabile rimozione). </summary>
        High,
        /// <summary> Priorità bassa (più probabile rimozione). </summary>
        Low
    }
}