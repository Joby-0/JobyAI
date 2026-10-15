namespace JobyAI.Api.Data.Models;

public sealed class ChatMessages
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string? Content { get; set; }
    public string? Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}