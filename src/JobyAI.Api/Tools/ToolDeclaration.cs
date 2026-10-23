using System.Text.Json;
using Microsoft.Extensions.AI;
using PersonalAssistant.Api.Tools;

namespace JobyAI.Api.Tools;

public sealed class ToolDeclaration : AIFunctionDeclaration
{
    private readonly JsonElement _schema;

    public ToolDeclaration(IAssistantTool tool)
    {
        Name = tool.Name;
        Description = tool.Description;

        using var document =
            JsonDocument.Parse(tool.InputSchemaJson);

        _schema = document.RootElement.Clone();
    }

    public override string Name { get; }

    public override string Description { get; }

    public override JsonElement JsonSchema => _schema;
}