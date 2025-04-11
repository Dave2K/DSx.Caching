namespace DSx.Caching.SharedKernel.Utilities
{
    /// <summary>
    /// Helper per la validazione degli input
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Verifica che un parametro non sia nullo
        /// </summary>
        /// <param name="parametro">Oggetto da verificare</param>
        /// <param name="nomeParametro">Nome del parametro per i messaggi</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AgainstNull(object? parametro, string nomeParametro)
        {
            if (parametro is null)
                throw new ArgumentNullException(nomeParametro);
        }
    }
}