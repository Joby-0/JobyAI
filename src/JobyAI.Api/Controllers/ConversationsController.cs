using Microsoft.AspNetCore.Mvc;
using JobyAI.Api.Services;
using JobyAI.Contracts.Conversations;

namespace JobyAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ConversationsController : ControllerBase
{
    private readonly IConversationService _conversationService;

    public ConversationsController(IConversationService conversationService)
    {
        _conversationService = conversationService ?? throw new ArgumentNullException(nameof(conversationService));
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ConversationSummary>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ConversationSummary>>> GetConversations(CancellationToken cancellationToken)
    {
        var conversations = await _conversationService.GetConversationsAsync(cancellationToken);
        return Ok(conversations);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ConversationDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversationDetails>> GetConversation(Guid id, CancellationToken cancellationToken)
    {
        var conversation = await _conversationService.GetConversationAsync(id, cancellationToken);
        return conversation is null ? NotFound() : Ok(conversation);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteConversation(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _conversationService.DeleteConversationAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
