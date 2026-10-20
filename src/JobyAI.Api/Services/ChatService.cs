using JobyAI.Api.AI;
using JobyAI.Api.Data.Models;
using JobyAI.Api.Data.Repos;
using Microsoft.Extensions.AI;

namespace JobyAI.Api.Services;

public sealed class ChatService : IChatService
{
    private readonly IAiProvider _aiProvider;

    private readonly ChatMessageRepo _chatMessageRepo;
    private readonly ConversationRepo _conversationRepo;

    public ChatService(IAiProvider aiProvider, ChatMessageRepo chatMessageRepo, ConversationRepo conversationRepo)
    {
        _aiProvider = aiProvider ?? throw new ArgumentNullException(nameof(aiProvider));
        _chatMessageRepo = chatMessageRepo ?? throw new ArgumentNullException(nameof(chatMessageRepo));
        _conversationRepo = conversationRepo ?? throw new ArgumentNullException(nameof(conversationRepo));
    }


    public async Task<string> GetResponseAsync(string message, Guid? conversationId = null, CancellationToken cancellationToken = default)
    {
        if (conversationId == null)
        {
            var conversation = await _conversationRepo.CreateAsync(new Conversations(), cancellationToken);

            conversationId = conversation.Id;
        }

        var chatMessages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are JobyAI, a helpful personal AI assistant.")
        };

        var existingMessages = await _chatMessageRepo.GetMessagesByConversationIdAsync(conversationId.Value, cancellationToken);

        chatMessages.AddRange(existingMessages);

        var userMessage = new ChatMessage(ChatRole.User, message);

        await _chatMessageRepo.SaveAsync(userMessage, conversationId.Value, cancellationToken);

        chatMessages.Add(userMessage);

        var response = await _aiProvider.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);

        var assistantMessage = new ChatMessage(ChatRole.Assistant, response.Text ?? string.Empty);

        await _chatMessageRepo.SaveAsync(assistantMessage, conversationId.Value, cancellationToken);

        return response.Text ?? string.Empty;
    }

}