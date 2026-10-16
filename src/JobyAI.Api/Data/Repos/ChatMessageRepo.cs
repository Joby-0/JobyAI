using JobyAI.Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace JobyAI.Api.Data.Repos;

public sealed class ChatMessageRepo
{
    private readonly MainDbContext _dbContext;

    public ChatMessageRepo(MainDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task SaveAsync(ChatMessage message, Guid conversationId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var entity = ConvertToChatMessageDB(message, conversationId);

        await _dbContext.ChatMessages.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<IEnumerable<ChatMessage>> GetMessagesByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.ChatMessages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ConvertToChatMessage).ToList();
    }

    private static ChatMessage ConvertToChatMessage(ChatMessageDB entity)
    {
        return new ChatMessage(new ChatRole(entity.Role), entity.Content)
        {
            MessageId = entity.Id.ToString(),
            CreatedAt = entity.CreatedAt
        };
    }

    private static ChatMessageDB ConvertToChatMessageDB(ChatMessage message, Guid conversationId)
    {
        return new ChatMessageDB
        {
            Id = Guid.TryParse(message.MessageId, out var id) ? id : Guid.NewGuid(),

            ConversationId = conversationId,
            Content = message.Text ?? string.Empty,
            Role = message.Role.Value,
            CreatedAt = message.CreatedAt ?? DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}