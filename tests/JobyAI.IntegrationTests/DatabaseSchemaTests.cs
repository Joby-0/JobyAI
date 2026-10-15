using JobyAI.Api.Data;
using JobyAI.Api.Data.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JobyAI.IntegrationTests;

public sealed class DatabaseSchemaTests
{
    [Fact]
    public async Task ModelCreatesRelationsAndCascadesConversationDeletes()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new MainDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var conversation = new Conversations
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var execution = new ToolExecutions
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Status = Status.Pending,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        context.Conversations.Add(conversation);
        context.ChatMessages.Add(new ChatMessages
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = "user",
            Content = "Hello",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        context.ToolExecutions.Add(execution);
        context.PendingApprovals.Add(new PendingApprovals
        {
            Id = Guid.NewGuid(),
            ToolExecutionId = execution.Id,
            Status = Status.Pending,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            ApprovedAt = DateTime.MinValue
        });

        await context.SaveChangesAsync();

        var persistedExecution = await context.ToolExecutions
            .Include(toolExecution => toolExecution.PendingApproval)
            .SingleAsync(toolExecution => toolExecution.Id == execution.Id);
        Assert.Equal(Status.Pending, persistedExecution.Status);
        Assert.Equal(Status.Pending, persistedExecution.PendingApproval!.Status);
        Assert.Equal(1, await context.ChatMessages.CountAsync());
        Assert.Equal(1, await context.PendingApprovals.CountAsync());

        context.Conversations.Remove(conversation);
        await context.SaveChangesAsync();

        Assert.Equal(0, await context.ChatMessages.CountAsync());
        Assert.Equal(0, await context.ToolExecutions.CountAsync());
        Assert.Equal(0, await context.PendingApprovals.CountAsync());
    }
}
