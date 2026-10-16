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

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string message)
    {
        var response = await _chatService.GetResponseAsync(message);
        return Ok(new { response });
    }
}
