using System.Security.Claims;
using Bomberman.Api.DTO;
using Bomberman.Api.Models;
using Bomberman.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bomberman.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/rooms")]
public sealed class GameRoomsController(GameRoomService gameRoomService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<GameRoomResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GameRoomResponseDto>> Create(
        CreateGameRoomRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetPlayerId(out var playerId))
        {
            return Unauthorized(new { message = "El token no contiene un jugador válido." });
        }

        var room = await gameRoomService.CreateAsync(
            request.Name,
            request.MaxPlayers,
            playerId,
            cancellationToken);

        if (room is null)
        {
            return Unauthorized(new { message = "El jugador del token ya no existe." });
        }

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, ToResponse(room));
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<GameRoomResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<GameRoomResponseDto>>> GetAvailable(
        CancellationToken cancellationToken)
    {
        var rooms = (await gameRoomService.GetAvailableAsync(cancellationToken))
            .Select(ToResponse)
            .ToList();

        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<GameRoomResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameRoomResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var room = await gameRoomService.GetByIdAsync(id, cancellationToken);
        return room is null ? NotFound() : Ok(ToResponse(room));
    }

    [HttpPost("{id:int}/join")]
    [ProducesResponseType<GameRoomResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GameRoomResponseDto>> Join(
        int id,
        CancellationToken cancellationToken)
    {
        if (!TryGetPlayerId(out var playerId))
        {
            return Unauthorized(new { message = "El token no contiene un jugador válido." });
        }

        var result = await gameRoomService.JoinAsync(id, playerId, cancellationToken);

        return result.Status switch
        {
            JoinGameRoomStatus.Success => Ok(ToResponse(result.Room!)),
            JoinGameRoomStatus.RoomNotFound => NotFound(new { message = "La sala no existe." }),
            JoinGameRoomStatus.PlayerNotFound => Unauthorized(
                new { message = "El jugador del token ya no existe." }),
            JoinGameRoomStatus.AlreadyJoined => Conflict(
                new { message = "El jugador ya pertenece a esta sala." }),
            JoinGameRoomStatus.RoomFull => Conflict(
                new { message = "La sala está llena." }),
            JoinGameRoomStatus.RoomNotWaiting => Conflict(
                new { message = "La sala ya no está esperando jugadores." }),
            _ => Conflict(
                new { message = "No se pudo completar la unión. Intente nuevamente." })
        };
    }

    private bool TryGetPlayerId(out int playerId)
    {
        return int.TryParse(User.FindFirstValue("player_id"), out playerId);
    }

    private static GameRoomResponseDto ToResponse(GameRoom room)
    {
        return new GameRoomResponseDto
        {
            Id = room.Id,
            Name = room.Name,
            MaxPlayers = room.MaxPlayers,
            Status = room.Status.ToString().ToLowerInvariant(),
            Participants = room.Participants
                .OrderBy(item => item.PlayerId)
                .Select(item => new RoomPlayerResponseDto
                {
                    PlayerId = item.PlayerId,
                    Name = item.Player.Nombre
                })
                .ToList()
        };
    }
}
