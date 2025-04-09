using DSx.Caching.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSx.Caching
{
    /// <summary>
    /// Factory per la creazione dinamica di provider di cache basati sulla configurazione
    /// </summary>
    /// <remarks>
    /// Gestisce il caricamento lazy dei provider e garantisce che solo i provider configurati vengano inizializzati
    /// </remarks>
    public class CacheProviderFactory
    {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        private readonly Dictionary<string, Lazy<ICacheProvider>> _providers = new();

        /// <summary>
        /// Inizializza una nuova istanza della factory
        /// </summary>
        /// <param name="config">Configurazione dell'applicazione</param>
        /// <param name="services">Provider di servizi DI</param>
        /// <exception cref="ArgumentNullException">
        /// Generata se <paramref name="config"/> o <paramref name="services"/> sono null
        /// </exception>
        public CacheProviderFactory(IConfiguration config, IServiceProvider services)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _services = services ?? throw new ArgumentNullException(nameof(services));

            InitializeProviders();
        }

        /// <summary>
        /// Carica i provider specificati nella configurazione
        /// </summary>
        private void InitializeProviders()
        {
            var providerNames = _config.GetSection("CacheSettings:Providers")
                .Get<string[]>()
                ?.Distinct(StringComparer.OrdinalIgnoreCase)
                ?? Array.Empty<string>();

            foreach (var name in providerNames)
            {
                _providers[name] = new Lazy<ICacheProvider>(() => CreateProviderInstance(name));
            }
        }

        /// <summary>
        /// Crea un'istanza del provider specificato
        /// </summary>
        /// <param name="providerName">Nome del provider da creare</param>
        /// <returns>Istanza del provider</returns>
        /// <exception cref="ProviderNotConfiguredException">
        /// Generata se il provider non è configurato correttamente
        /// </exception>
        private ICacheProvider CreateProviderInstance(string providerName)
        {
            return providerName.ToLowerInvariant() switch
            {
                "redis" => ActivatorUtilities.GetServiceOrCreateInstance<RedisCacheProvider>(_services),
                "memorycache" => ActivatorUtilities.GetServiceOrCreateInstance<MemoryCacheProvider>(_services),
                _ => throw new ProviderNotConfiguredException($"Provider '{providerName}' non supportato")
            };
        }

        /// <summary>
        /// Ottiene il provider di cache richiesto
        /// </summary>
        /// <param name="providerName">Nome del provider (opzionale, usa il default se omesso)</param>
        /// <returns>Istanza del provider configurato</returns>
        /// <exception cref="ProviderNotConfiguredException">
        /// Generata se il provider richiesto non è configurato
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Generata se non è definito un provider di default
        /// </exception>
        public ICacheProvider GetProvider(string? providerName = null)
        {
            var targetProvider = providerName?.Trim() ?? _config["CacheSettings:DefaultProvider"];

            if (string.IsNullOrWhiteSpace(targetProvider))
                throw new InvalidOperationException("Nessun provider di default configurato");

            if (_providers.TryGetValue(targetProvider, out var provider))
                return provider.Value;

            throw new ProviderNotConfiguredException($"Provider '{targetProvider}' non configurato");
        }
    }
}