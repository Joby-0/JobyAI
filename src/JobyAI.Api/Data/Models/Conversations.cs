namespace JobyAI.Api.Data.Models;

public sealed class Conversations
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<ChatMessages> Messages { get; set; } = new List<ChatMessages>();
    public ICollection<ToolExecutions> ToolExecutions { get; set; } = new List<ToolExecutions>();
}