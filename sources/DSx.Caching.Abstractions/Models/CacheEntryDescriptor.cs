using System;

namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Descrive i metadati di una voce nella cache
    /// </summary>
    public class CacheEntryDescriptor
    {
        /// <summary>
        /// Ottiene la chiave della voce nella cache
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Ottiene la data di creazione della voce
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Ottiene o imposta l'ultima data di accesso
        /// </summary>
        public DateTime LastAccessed { get; private set; }

        /// <summary>
        /// Ottiene la scadenza assoluta della voce
        /// </summary>
        public TimeSpan? AbsoluteExpiration { get; }

        /// <summary>
        /// Ottiene la scadenza relativa della voce
        /// </summary>
        public TimeSpan? SlidingExpiration { get; }

        /// <summary>
        /// Ottiene o imposta la dimensione in byte
        /// </summary>
        public long SizeInBytes { get; private set; }

        /// <summary>
        /// Indica se la voce è stata modificata
        /// </summary>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Contatore degli accessi in lettura
        /// </summary>
        public int ReadCount { get; private set; }

        /// <summary>
        /// Inizializza una nuova istanza della classe CacheEntryDescriptor
        /// </summary>
        /// <param name="key">Chiave della voce</param>
        /// <param name="createdAt">Data di creazione</param>
        /// <param name="lastAccessed">Ultimo accesso</param>
        /// <param name="absoluteExpiration">Scadenza assoluta</param>
        /// <param name="slidingExpiration">Scadenza relativa</param>
        /// <param name="sizeInBytes">Dimensione in byte</param>
        public CacheEntryDescriptor(
            string key,
            DateTime createdAt,
            DateTime lastAccessed,
            TimeSpan? absoluteExpiration,
            TimeSpan? slidingExpiration,
            long sizeInBytes)
        {
            Key = key;
            CreatedAt = createdAt;
            LastAccessed = lastAccessed;
            AbsoluteExpiration = absoluteExpiration;
            SlidingExpiration = slidingExpiration;
            SizeInBytes = sizeInBytes;
            IsDirty = false;
            ReadCount = 0;
        }

        /// <summary>
        /// Aggiorna i metadati dopo una lettura
        /// </summary>
        public void UpdateOnRead()
        {
            LastAccessed = DateTime.UtcNow;
            ReadCount++;
        }

        /// <summary>
        /// Aggiorna i metadati dopo una scrittura
        /// </summary>
        /// <param name="newSize">Nuova dimensione in byte</param>
        public void UpdateOnWrite(long newSize)
        {
            LastAccessed = DateTime.UtcNow;
            SizeInBytes = newSize;
            IsDirty = true;
        }

        /// <summary>
        /// Contrassegna la voce come pulita
        /// </summary>
        public void MarkAsClean()
        {
            IsDirty = false;
        }
    }
}
