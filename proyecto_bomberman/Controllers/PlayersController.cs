using Microsoft.AspNetCore.Mvc;
using Bomberman.Api.Models;
using Bomberman.Api.Services;
using Bomberman.Api.DTO;

namespace Bomberman.Api.Controllers;

[ApiController]
[Route("api/players")]
public sealed class PlayersController : ControllerBase
{
private readonly PlayerService _playerService;

    public PlayersController(PlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<PlayerResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PlayerResponseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var players = (await _playerService.GetAllAsync(cancellationToken))
            .Select(ToResponse)
            .ToList();
        return Ok(players);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<PlayerResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlayerResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var player = await _playerService.GetByIdAsync(id, cancellationToken);
        if (player is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(player));
    }

    [HttpPost]
    [ProducesResponseType<PlayerResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlayerResponseDto>> Create(
        CreatePlayerRequestDto player,
        CancellationToken cancellationToken)
    {
        var createdPlayer = await _playerService.CreateAsync(
            player.Nombre.Trim(),
            cancellationToken);
        var response = ToResponse(createdPlayer);
        return CreatedAtAction(
            nameof(GetById),
            new { id = createdPlayer.Id },
            response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<PlayerResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlayerResponseDto>> Replace(
        int id,
        ReplacePlayerRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!request.vida.HasValue)
        {
            ModelState.AddModelError(nameof(request.vida), "La vida es obligatoria.");
            return ValidationProblem(ModelState);
            //return BadRequest("La vida del jugador es obligatoria.");
        }

        if (!request.puntaje.HasValue)
        {
            ModelState.AddModelError(nameof(request.puntaje), "El puntaje es obligatorio.");
            return ValidationProblem(ModelState);
            //return BadRequest("El puntaje del jugador es obligatorio.");
        }

        var player = await _playerService.ReplaceAsync(
            id,
            request.Nombre.Trim(),
            request.vida.Value,
            request.puntaje.Value,
            cancellationToken);

        if (player is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(player));
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType<PlayerResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlayerResponseDto>> Update(
        int id,
        PatchPlayerRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.Nombre is null && request.vida is null && request.puntaje is null)
        {
            return BadRequest(new { message = "Debe proporcionar al menos un campo para actualizar." });
        }

        var player = await _playerService.UpdateAsync(
            id,
            request.Nombre?.Trim(),
            request.vida,
            request.puntaje,
            cancellationToken);

        if (player is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(player));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var estado = await _playerService.DeleteAsync(id, cancellationToken);
        if (!estado)
        {
            return NotFound();
        }
        
        return NoContent();
        //return Ok(new { message = "El jugador ha sido eliminado correctamente." });
    }

    private static PlayerResponseDto ToResponse(Player player)
    {
        return new PlayerResponseDto
        {
            //Id = player.Id,
            Nombre = player.Nombre,
            vida = player.vida,
            puntaje = player.puntaje
        };
    }
}
