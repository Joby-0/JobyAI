using JobyAI.Api.Agent;
using JobyAI.Api.AI;
using JobyAI.Api.Data.Models;
using JobyAI.Api.Data.Repos;
using JobyAI.Contracts.Chat;
using Microsoft.Extensions.AI;

namespace JobyAI.Api.Services;

public sealed class ChatService : IChatService
{
    private readonly IAgentOrchestrator _agentOrchestrator;

    private readonly ChatMessageRepo _chatMessageRepo;
    private readonly ConversationRepo _conversationRepo;

    public ChatService(IAgentOrchestrator agentOrchestrator, ChatMessageRepo chatMessageRepo, ConversationRepo conversationRepo)
    {
        _agentOrchestrator = agentOrchestrator ?? throw new ArgumentNullException(nameof(agentOrchestrator));
        _chatMessageRepo = chatMessageRepo ?? throw new ArgumentNullException(nameof(chatMessageRepo));
        _conversationRepo = conversationRepo ?? throw new ArgumentNullException(nameof(conversationRepo));
    }


    public async Task<string> GetResponseAsync(string message, Guid? conversationId = null, CancellationToken cancellationToken = default)
    {
        if (conversationId == null)
        {
            var conversation = await _conversationRepo.CreateAsync(new Conversations(), cancellationToken);

            conversationId = conversation.Id;
        }

        var chatMessages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are JobyAI, a helpful personal AI assistant.")
        };

        var existingMessages = await _chatMessageRepo.GetMessagesByConversationIdAsync(conversationId.Value, cancellationToken);

        chatMessages.AddRange(existingMessages);

        var userMessage = new ChatMessage(ChatRole.User, message);


        await _chatMessageRepo.SaveAsync(userMessage, conversationId.Value, cancellationToken);

        chatMessages.Add(userMessage);
        //start the loop in agent orchestrator to process the message and get the response

        //move this to agent orchestrator
        var result = await _agentOrchestrator.ProcessAsync(chatMessages, conversationId.Value, cancellationToken);

        var assistantMessage = new ChatMessage(ChatRole.Assistant, result.Response ?? string.Empty);

        if (result.Status == AgentTurnStatus.Completed)
        {
            await _chatMessageRepo.SaveAsync(assistantMessage, conversationId.Value, cancellationToken);
        }

        return result.Response ?? string.Empty;
    }

}