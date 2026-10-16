using JobyAI.Api.Data.Models;
using Microsoft.Extensions.AI;

namespace JobyAI.Api.AI;

public sealed class OllamaProvider : IAiProvider
{
    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessages> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
