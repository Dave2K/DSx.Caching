using DSx.Caching.Abstractions.Factories;
using StackExchange.Redis;

namespace DSx.Caching.Providers.Redis
{
    public class RedisConnectionMultiplexerFactory : IConnectionMultiplexerFactory
    {
        public IConnectionMultiplexer CreateConnection(string configurationString)
        {
            return ConnectionMultiplexer.Connect(
                ConfigurationOptions.Parse(configurationString)
            );
        }
    }
}