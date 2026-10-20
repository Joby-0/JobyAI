using JobyAI.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobyAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ChatController : ControllerBase
{
    readonly IChatService _chatService;
    public ChatController(IChatService chatService)
    {
        _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
    }

    /// <summary>
    /// Sends a chat message to the AI provider and returns the response.
    /// </summary>
    /// <param name="message">The chat message to send.</param>
    /// <returns>The response from the AI provider.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string message, [FromQuery] Guid? conversationId = null, CancellationToken cancellationToken = default)
    {
        var response = await _chatService.GetResponseAsync(message, conversationId, cancellationToken: cancellationToken);
        return Ok(new { response });
    }
}
