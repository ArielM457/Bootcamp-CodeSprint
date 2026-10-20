using CodeSprint.Session01.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CodeSprint.Session01.Api.Controllers;

[ApiController]
[Route("api/hello")]
public sealed class HelloController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<HelloResponse>(StatusCodes.Status200OK)]
    public ActionResult<HelloResponse> Get()
    {
        return Ok(new HelloResponse("¡Hola, CodeSprint!"));
    }
}
