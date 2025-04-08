namespace DSx.Caching.Abstractions.Exceptions
{
    using System;
    
    /// <summary>
    /// Base exception for all caching-related errors
    /// </summary>
    public class CacheException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheException"/> class
        /// </summary>
        /// <param name="message">A message describing the error context</param>
        /// <remarks>
        /// Used for non-recoverable errors without inner exception
        /// </remarks>
        public CacheException(string message) 
            : base(ValidateMessage(message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheException"/> class
        /// </summary>
        /// <param name="message">A message describing the error context</param>
        /// <param name="innerException">The original exception that caused the error</param>
        /// <remarks>
        /// Used to wrap exceptions from underlying systems (e.g., Redis, MemoryCache)
        /// </remarks>
        public CacheException(string message, Exception innerException)
            : base(ValidateMessage(message), innerException)
        {
        }

        /// <summary>
        /// Provides additional technical details about the error context
        /// </summary>
        public virtual string TechnicalDetails => 
            $"Cache Failure: {Message} | Type: {GetType().Name} | Stack: {StackTrace}";

        private static string ValidateMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(
                    "Exception message must contain meaningful information", 
                    nameof(message));

            return message.Trim();
        }
    }
}