using JobyAI.Api.AI;
using JobyAI.Api.Services;
using JobyAI.Contracts.Chat;
using Microsoft.AspNetCore.Mvc;

namespace JobyAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ChatController : ControllerBase
{
    readonly IChatService _chatService;
    readonly ILogger<ChatController> _logger;
    public ChatController(IChatService chatService, ILogger<ChatController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
    }

    /// <summary>
    /// Sends a chat message to the AI provider and returns the response.
    /// </summary>
    /// <param name="message">The chat message to send.</param>
    /// <param name="conversationId">An optional conversation ID to maintain context across multiple messages.</param>
    /// <returns>The response from the AI provider.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ChatRequest request, [FromQuery] Guid? conversationId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid message",
                Detail = "The message cannot be empty."
            });
        }

        try
        {
            var response = await _chatService.GetResponseAsync(request.Message, conversationId, cancellationToken: cancellationToken);

            return Ok(new { response });
        }
        catch (AiProviderException ex)
        {
            var statusCode = ex.Failure switch
            {
                AiProviderFailure.Timeout =>
                    StatusCodes.Status504GatewayTimeout,

                AiProviderFailure.OllamaUnavailable =>
                    StatusCodes.Status503ServiceUnavailable,

                AiProviderFailure.ModelMissing =>
                    StatusCodes.Status503ServiceUnavailable,

                _ => StatusCodes.Status502BadGateway
            };

            _logger.LogWarning(ex, "AI provider request failed. Failure: {Failure}", ex.Failure);

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "AI provider error",
                Detail = ex.Message,
                Instance = HttpContext.Request.Path
            };

            problem.Extensions["code"] = ex.Failure.ToString();
            problem.Extensions["traceId"] = HttpContext.TraceIdentifier;

            return new ObjectResult(problem)
            {
                StatusCode = statusCode
            };
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            // The client cancelled the request or disconnected.
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing chat request. " + "ConversationId: {ConversationId}", conversationId);

            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "An unexpected error occurred", detail: "The chat message could not be processed.");
        }
    }
}
