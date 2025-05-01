using DSx.Caching.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Resiliency
{
    /// <summary>
    /// Implementa il pattern Circuit Breaker per gestire errori temporanei nelle operazioni di cache
    /// </summary>
    public class CacheCircuitBreaker : ICacheCircuitBreaker
    {
        private readonly ILogger _logger;
        private readonly int _failureThreshold;
        private readonly TimeSpan _durationOfBreak;
        private int _failureCount;
        private DateTime _lastFailureTime;
        private CircuitState _state = CircuitState.Closed;

        /// <summary>
        /// Stato corrente del circuito
        /// </summary>
        public CircuitState State => _state;

        /// <summary>
        /// Crea una nuova istanza del Circuit Breaker
        /// </summary>
        /// <param name="logger">Logger per tracciare gli eventi</param>
        /// <param name="failureThreshold">Numero massimo di fallimenti prima di aprire il circuito</param>
        /// <param name="durationOfBreak">Durata dell'apertura del circuito</param>
        public CacheCircuitBreaker(
            ILogger<CacheCircuitBreaker> logger,
            int failureThreshold = 5,
            TimeSpan? durationOfBreak = null)
        {
            _logger = logger;
            _failureThreshold = failureThreshold;
            _durationOfBreak = durationOfBreak ?? TimeSpan.FromMinutes(1);
        }

        /// <summary>
        /// Esegue un'operazione con gestione degli errori tramite Circuit Breaker
        /// </summary>
        /// <typeparam name="T">Tipo del risultato</typeparam>
        /// <param name="action">Operazione principale da eseguire</param>
        /// <param name="fallbackAction">Azione di fallback da eseguire se il circuito è aperto</param>
        /// <param name="cancellationToken">Token di annullamento</param>
        public async Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> action,
            Func<CancellationToken, Task<T>> fallbackAction,
            CancellationToken cancellationToken = default)
        {
            if (_state == CircuitState.Open && DateTime.UtcNow < _lastFailureTime + _durationOfBreak)
            {
                _logger.LogWarning("Circuit is OPEN - Using fallback");
                return await fallbackAction(cancellationToken);
            }

            try
            {
                var result = await action(cancellationToken);
                Reset();
                return result;
            }
            catch (Exception ex)
            {
                TrackFailure(ex);
                throw;
            }
        }

        private void Reset()
        {
            _failureCount = 0;
            _state = CircuitState.Closed;
            _logger.LogInformation("Circuit RESET");
        }

        private void TrackFailure(Exception ex)
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;

            if (_failureCount >= _failureThreshold)
            {
                _state = CircuitState.Open;
                _logger.LogError(ex, "Circuit OPENED");
            }
            else
            {
                _state = CircuitState.HalfOpen;
                _logger.LogWarning("Circuit HALF-OPEN");
            }
        }
    }

    /// <summary>
    /// Stati possibili del Circuit Breaker
    /// </summary>
    public enum CircuitState
    {
        /// <summary> Circuito chiuso (operativo) </summary>
        Closed,
        /// <summary> Circuito semi-aperto (in fase di test) </summary>
        HalfOpen,
        /// <summary> Circuito aperto (bloccato) </summary>
        Open
    }
}
