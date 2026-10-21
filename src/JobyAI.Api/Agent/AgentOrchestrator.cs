namespace JobyAI.Api.Agent;

using System.Text.Json;
using Microsoft.Extensions.AI;
public sealed class AgentOrchestrator : IAgentOrchestrator
{
    public Task<AgentTurnResult> ProcessAsync(List<ChatMessage> messages, Guid conversationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
