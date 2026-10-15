using Microsoft.AspNetCore.Mvc;

namespace JobyAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ChatController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] string message)
    {
        // Handle the chat message here
        return Ok(new { response = $"Received message: {message}" });
    }
}
