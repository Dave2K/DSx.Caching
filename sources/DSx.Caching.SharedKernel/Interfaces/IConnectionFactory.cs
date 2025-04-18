// DSx.Caching.SharedKernel/Interfaces/IConnectionFactory.cs
namespace DSx.Caching.SharedKernel.Interfaces
{
    /// <summary>
    /// Interfaccia generica per la creazione di connessioni
    /// </summary>
    /// <typeparam name="TConnection">Tipo della connessione</typeparam>
    public interface IConnectionFactory<TConnection>
    {
        /// <summary>
        /// Crea una nuova connessione
        /// </summary>
        /// <param name="configuration">Configurazione specifica del provider</param>
        TConnection CreateConnection(object configuration);
    }
}