namespace JobyAI.Api.Services;

public interface IChatService
{
    Task<string> GetResponseAsync(string message, Guid? conversationId = null, CancellationToken cancellationToken = default);
}