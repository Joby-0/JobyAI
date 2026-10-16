using JobyAI.Api.AI;
using Microsoft.Extensions.AI;

namespace JobyAI.Api.Services;

public sealed class ChatService : IChatService
{
    private readonly IAiProvider _aiProvider;

    public ChatService(IAiProvider aiProvider)
    {
        _aiProvider = aiProvider ?? throw new ArgumentNullException(nameof(aiProvider));
    }

    public async Task<string> GetResponseAsync(string message, CancellationToken cancellationToken = default)
    {
        var chatMessages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, message)
        };

        var response = await _aiProvider.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);
        return response.Text;
    }
}