using JobyAI.Contracts.Conversations;

namespace JobyAI.Api.Services;

public interface IConversationService
{
    Task<IReadOnlyList<ConversationSummary>> GetConversationsAsync(CancellationToken cancellationToken = default);

    Task<ConversationDetails?> GetConversationAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteConversationAsync(Guid id, CancellationToken cancellationToken = default);
}
