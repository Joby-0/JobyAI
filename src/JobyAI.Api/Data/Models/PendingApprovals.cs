namespace JobyAI.Api.Data.Models;

public sealed class PendingApprovals
{
    public Guid Id { get; set; }
    public Guid ToolExecutionId { get; set; }
    public string? ActionSnapshotJson { get; set; }
    public Status Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime ApprovedAt { get; set; }

    public ToolExecutions ToolExecution { get; set; } = null!;
}