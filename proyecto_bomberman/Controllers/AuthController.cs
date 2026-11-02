using System.Security.Claims;
using Bomberman.Api.DTO;
using Bomberman.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bomberman.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(
        CreateUsuarioRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await authService.RegisterAsync(request, cancellationToken);

        if (response is null)
        {
            return Conflict(new { message = "Ya existe un usuario con ese correo." });
        }

        return CreatedAtAction(nameof(GetCurrentUser), response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);

        if (response is null)
        {
            return Unauthorized(new { message = "Correo o contraseña incorrectos." });
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult GetCurrentUser()
    {
        return Ok(new
        {
            UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            PlayerId = User.FindFirstValue("player_id"),
            Nombre = User.FindFirstValue(ClaimTypes.Name)
        });
    }
}
