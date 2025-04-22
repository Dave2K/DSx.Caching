using System;

namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Rappresenta i metadati di una voce nella cache
    /// </summary>
    public class CacheEntryDescriptor
    {
        /// <summary>
        /// Data e ora dell'ultimo accesso alla voce
        /// </summary>
        public DateTime LastAccessed { get; internal set; }

        /// <summary>
        /// Numero totale di letture effettuate
        /// </summary>
        public int ReadCount { get; internal set; }

        /// <summary>
        /// Dimensione occupata in memoria (in byte)
        /// </summary>
        public long SizeInBytes { get; internal set; }

        /// <summary>
        /// Indica se la voce ha modifiche non salvate
        /// </summary>
        public bool IsDirty { get; internal set; }

        /// <summary>
        /// Inizializza una nuova istanza della classe CacheEntryDescriptor
        /// </summary>
        /// <param name="lastAccessed">Data ultimo accesso</param>
        /// <param name="readCount">Contatore letture</param>
        /// <param name="sizeInBytes">Dimensione in byte</param>
        /// <param name="isDirty">Stato modifiche</param>
        public CacheEntryDescriptor(
            DateTime lastAccessed,
            int readCount,
            long sizeInBytes,
            bool isDirty)
        {
            LastAccessed = lastAccessed;
            ReadCount = readCount;
            SizeInBytes = sizeInBytes;
            IsDirty = isDirty;
        }

        /// <summary>
        /// Aggiorna i metadati dopo una lettura
        /// </summary>
        public void UpdateOnRead()
        {
            ReadCount++;
            LastAccessed = DateTime.UtcNow;
        }

        /// <summary>
        /// Aggiorna i metadati dopo una scrittura
        /// </summary>
        /// <param name="newSize">Nuova dimensione in byte</param>
        public void UpdateOnWrite(long newSize)
        {
            SizeInBytes = newSize;
            IsDirty = true;
            LastAccessed = DateTime.UtcNow;
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