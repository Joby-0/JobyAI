using JobyAI.Api.AI;
using JobyAI.Api.Data.Repos;
using Microsoft.Extensions.AI;

namespace JobyAI.Api.Services;

public sealed class ChatService : IChatService
{
    private readonly IAiProvider _aiProvider;

    private readonly ChatMessageRepo _chatMessageRepo;

    public ChatService(IAiProvider aiProvider, ChatMessageRepo chatMessageRepo)
    {
        _aiProvider = aiProvider ?? throw new ArgumentNullException(nameof(aiProvider));
        _chatMessageRepo = chatMessageRepo ?? throw new ArgumentNullException(nameof(chatMessageRepo));
    }

    public async Task<string> GetResponseAsync(string message, Guid? conversationId = null, CancellationToken cancellationToken = default)
    {
        var chatMessages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, message)
        };
        if (conversationId != null)
        {
            var existingMessages = await _chatMessageRepo.GetMessagesByConversationIdAsync(conversationId.Value, cancellationToken);
            chatMessages.AddRange(existingMessages);
        }


        var response = await _aiProvider.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);
        await _chatMessageRepo.SaveAsync(new ChatMessage(ChatRole.Assistant, response.Text), conversationId ?? Guid.NewGuid(), cancellationToken);
        return response.Text;
    }
}