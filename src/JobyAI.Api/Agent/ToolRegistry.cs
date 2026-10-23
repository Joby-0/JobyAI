using PersonalAssistant.Api.Tools;

namespace JobyAI.Api.Agent;

public sealed class ToolRegistry : IToolRegistry
{
    private readonly IReadOnlyCollection<IAssistantTool> _allTools;

    private readonly IReadOnlyDictionary<string, IAssistantTool> _tools;

    public ToolRegistry(IEnumerable<IAssistantTool> tools)
    {
        var registeredTools = tools.ToArray();

        var toolDictionary = new Dictionary<string, IAssistantTool>(StringComparer.OrdinalIgnoreCase);

        foreach (var tool in registeredTools)
        {
            if (string.IsNullOrWhiteSpace(tool.Name))
            {
                throw new InvalidOperationException("A registered tool must have a name.");
            }

            if (!toolDictionary.TryAdd(tool.Name, tool))
            {
                throw new InvalidOperationException($"A tool named '{tool.Name}' is already registered.");
            }
        }

        _allTools = Array.AsReadOnly(registeredTools);
        _tools = toolDictionary;
    }

    public IReadOnlyCollection<IAssistantTool> GetAll()
    {
        return _allTools;
    }

    public bool TryGet(string name, out IAssistantTool? tool)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            tool = null;
            return false;
        }

        return _tools.TryGetValue(name, out tool);
    }
}