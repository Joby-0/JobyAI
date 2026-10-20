namespace JobyAI.Api.AI;

public enum AiProviderFailure
{
    OllamaUnavailable,
    ModelMissing,
    Timeout,
    RequestFailed
}

public sealed class AiProviderException : Exception
{
    public AiProviderFailure Failure { get; }

    public AiProviderException(AiProviderFailure failure, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Failure = failure;
    }
}