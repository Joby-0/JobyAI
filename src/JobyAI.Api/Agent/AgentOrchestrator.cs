using System.Text.Json;
using JobyAI.Api.AI;
using JobyAI.Api.Tools;
using JobyAI.Contracts.Chat;
using Microsoft.Extensions.AI;

namespace JobyAI.Api.Agent;

public sealed class AgentOrchestrator : IAgentOrchestrator
{
    private const int MaxIterations = 6;

    private readonly IAiProvider _aiProvider;
    private readonly IToolRegistry _toolRegistry;
    private readonly IToolPermissionService _permissionService;
    private readonly IApprovalService _approvalService;
    private readonly ILogger<AgentOrchestrator> _logger;

    public AgentOrchestrator(IAiProvider aiProvider, IToolRegistry toolRegistry, IToolPermissionService permissionService, IApprovalService approvalService, ILogger<AgentOrchestrator> logger)
    {
        _aiProvider = aiProvider;
        _toolRegistry = toolRegistry;
        _permissionService = permissionService;
        _approvalService = approvalService;
        _logger = logger;
    }

    public async Task<AgentTurnResult> ProcessAsync(List<ChatMessage> messages, Guid conversationId, CancellationToken cancellationToken = default)
    {
        // Only expose tools registered by the application.
        var tools = _toolRegistry
            .GetAll()
            .Select(tool => (AITool)new ToolDeclaration(tool))
            .ToList();

        var options = new ChatOptions
        {
            Tools = tools,
            AllowMultipleToolCalls = false
        };

        for (var iteration = 0; iteration < MaxIterations; iteration++)
        {
            // 1. Ask the model to answer or request a tool.
            var response = await _aiProvider.GetResponseAsync(messages, options, cancellationToken);

            // 2. Preserve the model's structured response.
            messages.AddRange(response.Messages);

            var toolCalls = response.Messages
                .SelectMany(m => m.Contents)
                .OfType<FunctionCallContent>()
                .Where(call => !call.InformationalOnly)
                .ToList();

            // 3. No tool call means the model has finished.
            if (toolCalls.Count == 0)
            {
                return AgentTurnResult.Completed(conversationId, response.Text ?? string.Empty);
            }

            // The MVP processes one tool call per model response.
            if (toolCalls.Count > 1)
            {
                foreach (var call in toolCalls)
                {
                    AddToolResult(
                        messages,
                        call.CallId,
                        new
                        {
                            success = false,
                            error = "Please request only one tool at a time."
                        });
                }

                continue;
            }

            var toolCall = toolCalls[0];

            // 4. Resolve the tool from the trusted registry.
            if (!_toolRegistry.TryGet(toolCall.Name, out var tool) ||
                tool is null)
            {
                AddToolResult(
                    messages,
                    toolCall.CallId,
                    new
                    {
                        success = false,
                        error = "The requested tool is not available."
                    });

                continue;
            }

            // 5. Convert model arguments into JSON.
            var argumentsJson = JsonSerializer.Serialize(toolCall.Arguments ?? new Dictionary<string, object?>());

            using var argumentsDocument =JsonDocument.Parse(argumentsJson);

            var arguments = argumentsDocument.RootElement.Clone();

            // 6. Check the permission policy.
            var decision = await _permissionService.CheckAsync(
                tool,
                arguments,
                cancellationToken);

            if (decision.Action == PermissionAction.Deny)
            {
                AddToolResult(
                    messages,
                    toolCall.CallId,
                    new
                    {
                        success = false,
                        error = decision.Reason ?? "Permission denied."
                    });

                continue;
            }

            // 7. A sensitive action pauses for user approval.
            if (decision.Action == PermissionAction.RequiresApproval)
            {
                var approvalId =
                    await _approvalService.CreatePendingAsync(
                        conversationId,
                        tool.Name,
                        toolCall.CallId,
                        arguments,
                        messages,
                        cancellationToken);

                return AgentTurnResult.WaitingForApproval(
                    conversationId,
                    approvalId,
                    $"I need your approval before using {tool.Name}.");
            }

            // 8. Execute the approved-by-policy tool.
            ToolResult result;

            try
            {
                result = await tool.ExecuteAsync(arguments,
                    new ToolExecutionContext(
                        Guid.NewGuid(),
                        conversationId),
                    cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Tool {ToolName} failed.",
                    tool.Name);

                result = new ToolResult(false, "The tool failed to execute.");
            }

            // TODO: Persist the ToolExecution and its actual outcome.

            // 9. Feed the result back to the model.
            AddToolResult(
                messages,
                toolCall.CallId,
                new
                {
                    success = result.Succeeded,
                    message = result.Message,
                    data = result.DataJson
                });

            // The next iteration asks the model to interpret
            // the tool result, call another tool, or answer.
        }

        throw new InvalidOperationException($"The agent exceeded its limit of {MaxIterations} iterations.");
    }

    private static void AddToolResult(List<ChatMessage> messages, string callId, object result)
    {
        messages.Add(new ChatMessage(ChatRole.Tool,
            new List<AIContent>
            {
                new FunctionResultContent(callId, result)
            }));
    }
}