using Microsoft.Extensions.AI;

namespace JobyAI.Api.Agent;

public interface IAgentOrchestrator
{
    Task<AgentTurnResult> ProcessAsync(List<ChatMessage> messages, Guid conversationId, CancellationToken cancellationToken = default);
}