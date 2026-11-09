using System.Collections.Concurrent;
using Bomberman.Api.Data;
using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;
using Bomberman.Api.Enums;
namespace Bomberman.Api.Services;


public sealed class GameService(BombermanDbContext dbContext)
{
    private static readonly ConcurrentDictionary<Guid, GameSession> Games = new();

    private static readonly (int X, int Y)[] InitialPositions =
    [
        (1, 1),
        (7, 7),
        (7, 1),
        (1, 7),
        (3, 1),
        (5, 7),
        (5, 1),
        (3, 7)
    ];

    public async Task<StartGameResult> StartAsync(
        int roomId,
        int playerId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken);

        var room = await dbContext.GameRooms
            .FromSqlInterpolated(
                $"SELECT * FROM game_rooms WHERE id = {roomId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

        if (room is null)
        {
            return new StartGameResult(StartGameStatus.RoomNotFound);
        }

        await dbContext.Entry(room)
            .Collection(item => item.Participants)
            .LoadAsync(cancellationToken);

        if (!room.Participants.Any(item => item.PlayerId == playerId))
        {
            return new StartGameResult(StartGameStatus.PlayerNotInRoom);
        }

        if (room.Status != GameRoomStatus.Waiting)
        {
            return new StartGameResult(StartGameStatus.RoomNotWaiting);
        }

        var game = new GameSession
        {
            RoomId = room.Id,
            Players = room.Participants
                .OrderBy(item => item.PlayerId)
                .Select((item, index) => new PlayerPosition
                {
                    PlayerId = item.PlayerId,
                    X = InitialPositions[index].X,
                    Y = InitialPositions[index].Y
                })
                .ToList()
        };

        room.Status = GameRoomStatus.Playing;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        Games[game.Id] = game;
        return new StartGameResult(StartGameStatus.Success, game);
    }

    public GameSession? GetById(Guid gameId)
    {
        return Games.GetValueOrDefault(gameId);
    }

    public MovePlayerResult Move(Guid gameId, int playerId, string direction)
    {
        if (!Games.TryGetValue(gameId, out var game))
        {
            return new MovePlayerResult(MovePlayerStatus.GameNotFound);
        }

        lock (game)
        {
            if (game.Status == GameSessionStatus.Finished)
            {
                return new MovePlayerResult(MovePlayerStatus.GameFinished);
            }

            var player = game.Players.FirstOrDefault(item => item.PlayerId == playerId);
            if (player is null)
            {
                return new MovePlayerResult(MovePlayerStatus.PlayerNotInGame);
            }

            var movement = direction.Trim().ToLowerInvariant() switch
            {
                "up" => (X: 0, Y: -1),
                "down" => (X: 0, Y: 1),
                "left" => (X: -1, Y: 0),
                "right" => (X: 1, Y: 0),
                _ => (X: 0, Y: 0)
            };

            if (movement == (0, 0))
            {
                return new MovePlayerResult(MovePlayerStatus.InvalidDirection);
            }

            var newX = player.X + movement.X;
            var newY = player.Y + movement.Y;

            if (!game.Map.IsInside(newX, newY))
            {
                return new MovePlayerResult(MovePlayerStatus.OutsideMap);
            }

            if (!game.Map.IsFree(newX, newY))
            {
                return new MovePlayerResult(MovePlayerStatus.WallBlocked);
            }

            player.X = newX;
            player.Y = newY;
            return new MovePlayerResult(MovePlayerStatus.Success, game);
        }
    }
}
