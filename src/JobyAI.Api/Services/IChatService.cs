namespace JobyAI.Api.Services;

public interface IChatService
{
    Task<string> GetResponseAsync(string message, CancellationToken cancellationToken = default);
}