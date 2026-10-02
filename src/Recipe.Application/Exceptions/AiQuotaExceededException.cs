namespace Recipe.Application.Exceptions
{
    /// <summary>Thrown when the AI provider rejects a request because the account has no quota/credits left (e.g. HTTP 429).</summary>
    public class AiQuotaExceededException : Exception
    {
        public AiQuotaExceededException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
