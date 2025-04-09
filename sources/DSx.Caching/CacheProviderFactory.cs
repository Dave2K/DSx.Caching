using DSx.Caching.Abstractions.Interfaces;
using DSx.Caching.Providers.Memory;
using DSx.Caching.Providers.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace DSx.Caching
{
    /// <summary>
    /// Factory per la creazione dinamica dei provider di cache
    /// </summary>
    public class CacheProviderFactory
    {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        private readonly Dictionary<string, Lazy<ICacheProvider>> _providers = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Inizializza una nuova istanza della factory
        /// </summary>
        /// <param name="config">Configurazione dell'applicazione</param>
        /// <param name="services">Provider di servizi DI</param>
        public CacheProviderFactory(IConfiguration config, IServiceProvider services)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            InitializeProviders();
        }

        /// <summary>
        /// Ottiene un provider di cache per nome
        /// </summary>
        /// <param name="providerName">Nome del provider (case-insensitive)</param>
        /// <returns>Istanza del provider configurato</returns>
        /// <exception cref="ProviderNotConfiguredException">Se il provider non è configurato</exception>
        public ICacheProvider GetProvider(string? providerName = null)
        {
            var targetProvider = providerName?.Trim() ?? _config["CacheSettings:DefaultProvider"];

            if (string.IsNullOrWhiteSpace(targetProvider))
                throw new InvalidOperationException("Nessun provider predefinito configurato");

            if (_providers.TryGetValue(targetProvider, out var provider))
                return provider.Value;

            throw new ProviderNotConfiguredException($"Provider '{targetProvider}' non configurato");
        }

        private void InitializeProviders()
        {
            var providerSection = _config.GetSection("CacheSettings:Providers");
            var providers = providerSection?.Get<string[]>() ?? Array.Empty<string>();

            foreach (var provider in providers)
            {
                _providers[provider] = new Lazy<ICacheProvider>(() => CreateProvider(provider));
            }
        }

        private ICacheProvider CreateProvider(string providerName)
        {
            return providerName.ToLowerInvariant() switch
            {
                "redis" => ActivatorUtilities.GetServiceOrCreateInstance<RedisCacheProvider>(_services),
                "memorycache" => ActivatorUtilities.GetServiceOrCreateInstance<MemoryCacheProvider>(_services),
                _ => throw new ProviderNotConfiguredException($"Provider '{providerName}' non supportato")
            };
        }
    }
}