namespace JobyAI.Api.Data.Models;

public sealed class ChatMessageDB
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string? Content { get; set; }
    public string? Role { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Conversations Conversation { get; set; } = null!;
}