using JobyAI.Api.Data.Models;
using Microsoft.Extensions.AI;

namespace JobyAI.Api.AI;

public interface IAiProvider
{
    Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessages> messages, ChatOptions? options = null, CancellationToken cancellationToken = default);
}
