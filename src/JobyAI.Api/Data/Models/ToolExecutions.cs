namespace JobyAI.Api.Data.Models;

public sealed class ToolExecutions
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string? ToolName { get; set; }
    public string? ResultSummary { get; set; }
    public string? ArgumentsJson { get; set; }
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime CompletedAt { get; set; }
}