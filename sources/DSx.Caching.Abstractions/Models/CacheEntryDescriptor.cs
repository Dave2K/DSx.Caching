using System;

namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Rappresenta i metadati e le informazioni di stato per una voce nella cache
    /// </summary>
    /// <remarks>
    /// Contiene informazioni dettagliate sullo stato della cache e metriche di utilizzo
    /// </remarks>
    public class CacheEntryDescriptor
    {
        /// <summary>
        /// Data e ora dell'ultimo accesso in lettura alla voce
        /// </summary>
        /// <value>DateTime in formato UTC</value>
        public DateTime LastAccessed { get; internal set; }

        /// <summary>
        /// Numero totale di accessi in lettura alla voce
        /// </summary>
        /// <value>Contatore intero non negativo</value>
        public int ReadCount { get; internal set; }

        /// <summary>
        /// Dimensione approssimativa della voce in memoria
        /// </summary>
        /// <value>Dimensione in bytes</value>
        /// <remarks>
        /// Il calcolo può variare in base al provider di cache
        /// </remarks>
        public long SizeInBytes { get; internal set; }

        /// <summary>
        /// Indica se la voce è stata modificata dopo l'inserimento iniziale
        /// </summary>
        /// <value>True se modificata, altrimenti False</value>
        public bool IsDirty { get; internal set; }

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="CacheEntryDescriptor"/>
        /// </summary>
        /// <param name="lastAccessed">Data ultimo accesso</param>
        /// <param name="readCount">Contatore letture</param>
        /// <param name="sizeInBytes">Dimensione in bytes</param>
        /// <param name="isDirty">Stato modifica</param>
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
        /// Aggiorna i metadati dopo un'operazione di lettura
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Sollevata se si tenta un aggiornamento non valido
        /// </exception>
        public void UpdateOnRead()
        {
            ReadCount++;
            LastAccessed = DateTime.UtcNow;
        }

        /// <summary>
        /// Aggiorna i metadati dopo un'operazione di scrittura
        /// </summary>
        /// <param name="newSize">Nuova dimensione in bytes</param>
        public void UpdateOnWrite(long newSize)
        {
            SizeInBytes = newSize;
            IsDirty = true;
            LastAccessed = DateTime.UtcNow;
        }

        /// <summary>
        /// Resetta lo stato di modifica della voce
        /// </summary>
        public void MarkAsClean()
        {
            IsDirty = false;
        }
    }
}