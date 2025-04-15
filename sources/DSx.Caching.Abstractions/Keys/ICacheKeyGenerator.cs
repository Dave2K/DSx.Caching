namespace DSx.Caching.Abstractions.Keys
{
    /// <summary>
    /// Provides cache key generation and normalization services
    /// </summary>
    public interface ICacheKeyGenerator
    {
        /// <summary>
        /// Generates a standardized cache key from input parameters
        /// </summary>
        /// <param name="baseKey">Root key identifier</param>
        /// <param name="parameters">Additional key components</param>
        /// <returns>Normalized cache key</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when baseKey is null or empty</exception>
        string GenerateKey(string baseKey, object[] parameters);

        /// <summary>
        /// Standardizes raw key formats for cache storage
        /// </summary>
        /// <param name="rawKey">Original key value</param>
        /// <returns>Sanitized key value</returns>
        /// <exception cref="System.ArgumentException">Thrown for invalid key formats</exception>
        string NormalizeKey(string rawKey);
    }
}