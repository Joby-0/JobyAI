using System.Text.Json;
using OllamaSharp.Tools;

namespace PersonalAssistant.Api.Tools;

public interface IAssistantTool
{
    string Name { get; }

    string Description { get; }

    string InputSchemaJson { get; }

    Task<ToolResult> ExecuteAsync(JsonElement arguments, ToolExecutionContext context,CancellationToken cancellationToken);
}