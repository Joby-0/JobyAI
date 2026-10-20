using JobyAI.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace JobyAI.Api.Data.Repos;

public sealed class ConversationRepo
{
    private readonly MainDbContext _dbContext;

    public ConversationRepo(MainDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<Conversations>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .AsNoTracking()
            .OrderByDescending(conversation => conversation.UpdatedAt)
            .ThenByDescending(conversation => conversation.CreatedAt)
            .ToListAsync(cancellationToken);
    }
    public async Task<Conversations> CreateAsync(Conversations conversation, CancellationToken cancellationToken = default)
    {
        if (conversation is null)
        {
            throw new ArgumentNullException(nameof(conversation));
        }

        _dbContext.Conversations.Add(conversation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return conversation;
    }

    public Task<Conversations?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Conversations
            .AsNoTracking()
            .Include(conversation => conversation.Messages)
            .FirstOrDefaultAsync(conversation => conversation.Id == id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var conversation = await _dbContext.Conversations
            .FirstOrDefaultAsync(conversation => conversation.Id == id, cancellationToken);

        if (conversation is null)
        {
            return false;
        }

        _dbContext.Conversations.Remove(conversation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
