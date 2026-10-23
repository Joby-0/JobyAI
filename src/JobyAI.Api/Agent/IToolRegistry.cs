
using JobyAI.Api.Tools;

namespace JobyAI.Api.Agent;

public interface IToolRegistry
{
    IReadOnlyCollection<IAssistantTool> GetAll();

    bool TryGet(string name, out IAssistantTool? tool);
}