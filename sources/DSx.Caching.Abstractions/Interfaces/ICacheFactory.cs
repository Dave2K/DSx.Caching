using System.Collections.Generic;

namespace DSx.Caching.Abstractions.Interfaces
{
    /// <summary>
    /// Factory for creating cache provider instances
    /// </summary>
    public interface ICacheFactory
    {
        /// <summary>
        /// Creates a cache provider by name
        /// </summary>
        /// <param name="name">Provider name</param>
        /// <returns>Cache provider instance</returns>
        ICacheProvider CreateProvider(string name);

        /// <summary>
        /// Gets all registered provider names
        /// </summary>
        IEnumerable<string> GetProviderNames();
    }
}