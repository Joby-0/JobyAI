using Microsoft.AspNetCore.Mvc;

namespace JobyAI.Api.Controllers;

[ApiController]
[Route("/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> Get()
    {
        return Ok(new HealthResponse("healthy"));
    }

    /// <summary>Describes the current health of the API process.</summary>
    public sealed record HealthResponse(string Status);
}
