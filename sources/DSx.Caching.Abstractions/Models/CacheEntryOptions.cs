namespace DSx.Caching.Abstractions.Models
{
    /// <summary>
    /// Represents configuration options for a cache entry
    /// </summary>
    public class CacheEntryOptions
    {
        /// <summary>
        /// Absolute expiration time relative to now
        /// </summary>
        public TimeSpan? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Sliding expiration window
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// Priority level for cache retention
        /// </summary>
        public CacheEntryPriority Priority { get; set; } = CacheEntryPriority.Normal;
    }

    /// <summary>
    /// Specifies priority levels for cache entry retention
    /// </summary>
    public enum CacheEntryPriority
    {
        /// <summary>Low priority</summary>
        Low,
        /// <summary>Normal priority</summary>
        Normal,
        /// <summary>High priority</summary>
        High,
        /// <summary>Never remove unless explicitly deleted</summary>
        NeverRemove
    }
}
