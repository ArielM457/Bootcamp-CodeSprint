using System.Security.Claims;
using Bomberman.Api.DTO;
using Bomberman.Api.Enums;
using Bomberman.Api.Models;
using Bomberman.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bomberman.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/games")]
public sealed class GamesController(GameService gameService) : ControllerBase
{
    [HttpPost("{roomId:int}/start")]
    [ProducesResponseType<GameSessionResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GameSessionResponseDto>> Start(
        int roomId,
        CancellationToken cancellationToken)
    {
        if (!TryGetPlayerId(out var playerId))
        {
            return Unauthorized(new { message = "El token no contiene un jugador válido." });
        }

        var result = await gameService.StartAsync(roomId, playerId, cancellationToken);

        return result.Status switch
        {
            StartGameStatus.Success => CreatedAtAction(
                nameof(GetById),
                new { gameId = result.Game!.Id },
                GameSessionMapper.ToResponse(result.Game)),
            StartGameStatus.RoomNotFound => NotFound(
                new { message = "La sala no existe." }),
            StartGameStatus.PlayerNotInRoom => StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = "El jugador no pertenece a esta sala." }),
            StartGameStatus.PlayerNotHost => StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = "Solo el creador puede iniciar la partida." }),
            StartGameStatus.NotEnoughPlayers => Conflict(
                new { message = "Se necesitan al menos 2 jugadores para iniciar." }),
            StartGameStatus.PlayersNotReady => Conflict(
                new { message = "Todos los demás jugadores deben estar listos." }),
            StartGameStatus.RoomNotWaiting => Conflict(
                new { message = "La sala ya inició o terminó su partida." }),
            _ => Conflict(new { message = "No se pudo iniciar la partida." })
        };
    }

    [HttpGet("{gameId:guid}")]
    [ProducesResponseType<GameSessionResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameSessionResponseDto> GetById(Guid gameId)
    {
        if (!TryGetPlayerId(out var playerId))
        {
            return Unauthorized(new { message = "El token no contiene un jugador válido." });
        }

        var game = gameService.GetById(gameId);
        if (game is null)
        {
            return NotFound(new { message = "La partida no existe." });
        }

        if (game.Players.All(item => item.PlayerId != playerId))
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = "El jugador no pertenece a esta partida." });
        }

        return Ok(GameSessionMapper.ToResponse(game));
    }

    [HttpPost("{gameId:guid}/move")]
    [ProducesResponseType<GameSessionResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<GameSessionResponseDto> Move(
        Guid gameId,
        MovePlayerRequestDto request)
    {
        if (!TryGetPlayerId(out var playerId))
        {
            return Unauthorized(new { message = "El token no contiene un jugador válido." });
        }

        var result = gameService.Move(gameId, playerId, request.Direction);

        return result.Status switch
        {
            MovePlayerStatus.Success => Ok(GameSessionMapper.ToResponse(result.Game!)),
            MovePlayerStatus.GameNotFound => NotFound(
                new { message = "La partida no existe." }),
            MovePlayerStatus.PlayerNotInGame => StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = "El jugador no pertenece a esta partida." }),
            MovePlayerStatus.GameFinished => Conflict(
                new { message = "La partida ya terminó." }),
            MovePlayerStatus.InvalidDirection => BadRequest(
                new { message = "La dirección debe ser up, down, left o right." }),
            MovePlayerStatus.OutsideMap => Conflict(
                new { message = "El jugador no puede salir del mapa." }),
            MovePlayerStatus.WallBlocked => Conflict(
                new { message = "Una pared bloquea el movimiento." }),
            MovePlayerStatus.BombBlocked => Conflict(
                new { message = "Una bomba bloquea el movimiento." }),
            MovePlayerStatus.MoveTooSoon => Conflict(
                new { message = "Espera un momento antes de volver a moverte." }),
            MovePlayerStatus.PlayerEliminated => Conflict(
                new { message = "El jugador está eliminado." }),
            _ => Conflict(new { message = "No se pudo procesar el movimiento." })
        };
    }

    private bool TryGetPlayerId(out int playerId)
    {
        return int.TryParse(User.FindFirstValue("player_id"), out playerId);
    }

}
