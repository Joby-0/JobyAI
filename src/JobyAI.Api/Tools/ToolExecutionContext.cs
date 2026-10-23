namespace JobyAI.Api.Tools;
public sealed record ToolExecutionContext(
    Guid ToolExecutionId,
    Guid ConversationId);