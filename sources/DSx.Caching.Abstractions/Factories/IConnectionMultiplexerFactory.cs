using StackExchange.Redis;

namespace DSx.Caching.Abstractions.Factories
{
    /// <summary>
    /// Factory per la creazione di connessioni a Redis
    /// </summary>
    public interface IConnectionMultiplexerFactory
    {
        /// <summary>
        /// Crea una connessione a Redis
        /// </summary>
        /// <param name="configurationString">Stringa di configurazione Redis</param>
        IConnectionMultiplexer CreateConnection(string configurationString);
    }
}