using JobyAI.Api.Data.Models;
using Microsoft.Extensions.AI;

namespace JobyAI.Api.AI;

public sealed class OllamaProvider : IAiProvider
{
    private readonly IChatClient _chatClient;

    public OllamaProvider(IChatClient chatClient)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    }
    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _chatClient.GetResponseAsync(messages, options, cancellationToken);
    }
}
