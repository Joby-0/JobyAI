namespace JobyAI.Api.Data.Models;

public sealed class ChatMessageDB
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string? Content { get; set; }
    public string? Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Conversations Conversation { get; set; } = null!;
}