namespace JobyAI.Contracts.Chat;

public sealed record ChatRequest(string Message);

public enum AgentTurnStatus
{
    Completed,
    WaitingForApproval
}

public sealed record AgentTurnResult(Guid ConversationId, AgentTurnStatus Status, string? Response = null, Guid? ApprovalId = null)
{
    public static AgentTurnResult Completed(Guid conversationId, string response)
    {
        return new AgentTurnResult(conversationId, AgentTurnStatus.Completed, Response: response);
    }

    public static AgentTurnResult WaitingForApproval(Guid conversationId, Guid approvalId, string? response = null)
    {
        return new AgentTurnResult(conversationId, AgentTurnStatus.WaitingForApproval, Response: response, ApprovalId: approvalId);
    }
}