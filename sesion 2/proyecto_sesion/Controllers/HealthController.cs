using CodeSprint.Session02.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeSprint.Session02.Api.Controllers;

[ApiController]
public sealed class HealthController : ControllerBase
{
    [HttpGet("health")]
    [ProducesResponseType<HealthResponse>(StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> Get()
    {
        return Ok(new HealthResponse { Status = "OK" });
    }
}
