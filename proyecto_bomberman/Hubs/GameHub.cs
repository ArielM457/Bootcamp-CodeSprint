using System.Security.Claims;
using Bomberman.Api.DTO;
using Bomberman.Api.Enums;
using Bomberman.Api.Models;
using Bomberman.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Bomberman.Api.Hubs;

[Authorize]
public sealed class GameHub(GameService gameService) : Hub
{
    private const string GameIdItem = "GameId";
    private const string RoomIdItem = "RoomId";
    private const string PlayerIdItem = "PlayerId";

    public async Task JoinGame(Guid gameId)
    {
        var playerId = GetPlayerId();

        if (Context.Items.ContainsKey(GameIdItem))
        {
            throw new HubException("La conexión ya pertenece a una partida.");
        }

        var game = gameService.GetById(gameId);
        if (game is null)
        {
            throw new HubException("La partida no existe.");
        }

        var playerBelongsToGame = game.Players.Any(player => player.PlayerId == playerId);
        if (!playerBelongsToGame)
        {
            throw new HubException("El jugador no pertenece a esta sala.");
        }

        var groupName = GetGroupName(game.RoomId);
        var gameState = ToResponse(game);

        Context.Items[GameIdItem] = game.Id;
        Context.Items[RoomIdItem] = game.RoomId;
        Context.Items[PlayerIdItem] = playerId;

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("PlayerConnected", playerId);
        await Clients.Caller.SendAsync("GameStateUpdated", gameState);
    }

    public async Task MovePlayer(string direction)
    {
        var playerId = GetPlayerId();

        if (!Context.Items.TryGetValue(GameIdItem, out var gameIdValue) ||
            gameIdValue is not Guid gameId ||
            !Context.Items.TryGetValue(RoomIdItem, out var roomIdValue) ||
            roomIdValue is not int roomId)
        {
            throw new HubException("Primero debes conectarte a una partida.");
        }

        var result = gameService.Move(gameId, playerId, direction);
        if (result.Status != MovePlayerStatus.Success)
        {
            throw new HubException(GetMoveErrorMessage(result.Status));
        }

        var game = result.Game!;
        var player = game.Players.First(item => item.PlayerId == playerId);
        var gameState = ToResponse(game);
        var groupName = GetGroupName(roomId);

        await Clients.Group(groupName).SendAsync(
            "PlayerMoved",
            player.PlayerId,
            player.X,
            player.Y);
        await Clients.Group(groupName).SendAsync("GameStateUpdated", gameState);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue(RoomIdItem, out var roomIdValue) &&
            roomIdValue is int roomId &&
            Context.Items.TryGetValue(PlayerIdItem, out var playerIdValue) &&
            playerIdValue is int playerId)
        {
            var groupName = GetGroupName(roomId);
            await Clients.Group(groupName).SendAsync("PlayerDisconnected", playerId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private int GetPlayerId()
    {
        var playerIdClaim = Context.User?.FindFirstValue("player_id");
        if (!int.TryParse(playerIdClaim, out var playerId))
        {
            throw new HubException("El token no contiene un jugador válido.");
        }

        return playerId;
    }

    private static string GetGroupName(int roomId)
    {
        return $"room-{roomId}";
    }

    private static string GetMoveErrorMessage(MovePlayerStatus status)
    {
        return status switch
        {
            MovePlayerStatus.GameNotFound => "La partida no existe.",
            MovePlayerStatus.PlayerNotInGame => "El jugador no pertenece a esta partida.",
            MovePlayerStatus.GameFinished => "La partida ya terminó.",
            MovePlayerStatus.InvalidDirection => "La dirección debe ser up, down, left o right.",
            MovePlayerStatus.OutsideMap => "El jugador no puede salir del mapa.",
            MovePlayerStatus.WallBlocked => "Una pared bloquea el movimiento.",
            _ => "No se pudo procesar el movimiento."
        };
    }

    private static GameSessionResponseDto ToResponse(GameSession game)
    {
        return new GameSessionResponseDto
        {
            Id = game.Id,
            RoomId = game.RoomId,
            Status = game.Status.ToString().ToLowerInvariant(),
            Map = game.Map.Cells
                .Select(row => row.Select(cell => (int)cell).ToArray())
                .ToArray(),
            Players = game.Players
                .OrderBy(player => player.PlayerId)
                .Select(player => new PlayerPositionResponseDto
                {
                    PlayerId = player.PlayerId,
                    X = player.X,
                    Y = player.Y
                })
                .ToList()
        };
    }
}
