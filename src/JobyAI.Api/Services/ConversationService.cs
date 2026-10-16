using JobyAI.Api.Data.Repos;
using JobyAI.Contracts.Conversations;

namespace JobyAI.Api.Services;

public sealed class ConversationService : IConversationService
{
    private readonly ConversationRepo _conversationRepo;

    public ConversationService(ConversationRepo conversationRepo)
    {
        _conversationRepo = conversationRepo ?? throw new ArgumentNullException(nameof(conversationRepo));
    }

    public async Task<IReadOnlyList<ConversationSummary>> GetConversationsAsync(CancellationToken cancellationToken = default)
    {
        var conversations = await _conversationRepo.GetAllAsync(cancellationToken);
        return conversations
            .Select(conversation => new ConversationSummary(
                conversation.Id,
                conversation.Title,
                conversation.CreatedAt,
                conversation.UpdatedAt))
            .ToList();
    }

    public async Task<ConversationDetails?> GetConversationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepo.GetByIdAsync(id, cancellationToken);
        if (conversation is null)
        {
            return null;
        }

        var messages = conversation.Messages
            .OrderBy(message => message.CreatedAt)
            .ThenBy(message => message.Id)
            .Select(message => new ConversationMessage(
                message.Id,
                message.Role,
                message.Content,
                message.CreatedAt))
            .ToList();

        return new ConversationDetails(
            conversation.Id,
            conversation.Title,
            conversation.CreatedAt,
            conversation.UpdatedAt,
            messages);
    }

    public Task<bool> DeleteConversationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _conversationRepo.DeleteAsync(id, cancellationToken);
    }
}
