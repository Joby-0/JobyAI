using JobyAI.Api.Data;
using JobyAI.Api.Data.Models;
using JobyAI.Api.Data.Repos;
using JobyAI.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JobyAI.IntegrationTests;

public sealed class ConversationServiceTests
{
    [Fact]
    public async Task ListsGetsAndDeletesConversations()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new MainDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var olderConversation = new Conversations
        {
            Id = Guid.NewGuid(),
            Title = "Older",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        var conversation = new Conversations
        {
            Id = Guid.NewGuid(),
            Title = "Current",
            CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
        };
        context.Conversations.AddRange(olderConversation, conversation);
        context.ChatMessages.AddRange(
            new ChatMessageDB
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                Role = "assistant",
                Content = "Reply",
                CreatedAt = DateTimeOffset.Parse("2026-01-02T00:00:02Z").UtcDateTime
            },
            new ChatMessageDB
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                Role = "user",
                Content = "Hello",
                CreatedAt = DateTimeOffset.Parse("2026-01-02T00:00:01Z").UtcDateTime
            });
        await context.SaveChangesAsync();

        var service = new ConversationService(new ConversationRepo(context));

        var list = await service.GetConversationsAsync();
        Assert.Equal(new[] { conversation.Id, olderConversation.Id }, list.Select(item => item.Id));

        var details = await service.GetConversationAsync(conversation.Id);
        Assert.NotNull(details);
        Assert.Equal("Current", details.Title);
        Assert.Equal(new[] { "Hello", "Reply" }, details.Messages.Select(message => message.Content));
        Assert.Null(await service.GetConversationAsync(Guid.NewGuid()));

        Assert.True(await service.DeleteConversationAsync(conversation.Id));
        Assert.False(await service.DeleteConversationAsync(conversation.Id));
        Assert.Equal(0, await context.ChatMessages.CountAsync(message => message.ConversationId == conversation.Id));
        Assert.Null(await context.Conversations.FindAsync(conversation.Id));
    }
}
