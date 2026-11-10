using System.Collections.Concurrent;
using Bomberman.Api.Data;
using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;
using Bomberman.Api.Enums;
namespace Bomberman.Api.Services;


public sealed class GameService(BombermanDbContext dbContext)
{
    public static readonly TimeSpan BombFuse = TimeSpan.FromSeconds(2);
    public static readonly TimeSpan ExplosionDuration = TimeSpan.FromMilliseconds(650);
    public static readonly TimeSpan MovementCooldown = TimeSpan.FromMilliseconds(180);
    private static readonly ConcurrentDictionary<Guid, GameSession> Games = new();
    private static readonly ConcurrentDictionary<int, Guid> GamesByRoom = new();

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

        if (room.HostPlayerId != playerId)
        {
            return new StartGameResult(StartGameStatus.PlayerNotHost);
        }

        if (room.Status != GameRoomStatus.Waiting)
        {
            return new StartGameResult(StartGameStatus.RoomNotWaiting);
        }

        if (room.Participants.Count < 2)
        {
            return new StartGameResult(StartGameStatus.NotEnoughPlayers);
        }

        if (room.Participants.Any(item => item.PlayerId != room.HostPlayerId && !item.IsReady))
        {
            return new StartGameResult(StartGameStatus.PlayersNotReady);
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
        GamesByRoom[room.Id] = game.Id;
        return new StartGameResult(StartGameStatus.Success, game);
    }

    public GameSession? GetById(Guid gameId)
    {
        return Games.GetValueOrDefault(gameId);
    }

    public GameSession? GetByRoomId(int roomId)
    {
        return GamesByRoom.TryGetValue(roomId, out var gameId)
            ? GetById(gameId)
            : null;
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

            if (!player.IsAlive)
            {
                return new MovePlayerResult(MovePlayerStatus.PlayerEliminated);
            }

            var now = DateTimeOffset.UtcNow;
            if (now - player.LastMovedAtUtc < MovementCooldown)
            {
                return new MovePlayerResult(MovePlayerStatus.MoveTooSoon);
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

            if (game.Bombs.Any(bomb => bomb.X == newX && bomb.Y == newY))
            {
                return new MovePlayerResult(MovePlayerStatus.BombBlocked);
            }

            player.X = newX;
            player.Y = newY;
            player.LastMovedAtUtc = now;
            return new MovePlayerResult(MovePlayerStatus.Success, game);
        }
    }

    public PlaceBombResult PlaceBomb(Guid gameId, int playerId)
    {
        if (!Games.TryGetValue(gameId, out var game))
        {
            return new PlaceBombResult(PlaceBombStatus.GameNotFound);
        }

        lock (game)
        {
            if (game.Status == GameSessionStatus.Finished)
            {
                return new PlaceBombResult(PlaceBombStatus.GameFinished);
            }

            var player = game.Players.FirstOrDefault(item => item.PlayerId == playerId);
            if (player is null)
            {
                return new PlaceBombResult(PlaceBombStatus.PlayerNotInGame);
            }

            if (!player.IsAlive)
            {
                return new PlaceBombResult(PlaceBombStatus.PlayerEliminated);
            }

            if (game.Bombs.Any(item => item.OwnerPlayerId == playerId))
            {
                return new PlaceBombResult(PlaceBombStatus.PlayerAlreadyHasBomb);
            }

            if (game.Bombs.Any(item => item.X == player.X && item.Y == player.Y))
            {
                return new PlaceBombResult(PlaceBombStatus.CellAlreadyHasBomb);
            }

            var now = DateTimeOffset.UtcNow;
            var bomb = new GameBomb
            {
                OwnerPlayerId = playerId,
                X = player.X,
                Y = player.Y,
                PlacedAtUtc = now,
                ExplodesAtUtc = now.Add(BombFuse)
            };
            game.Bombs.Add(bomb);
            return new PlaceBombResult(PlaceBombStatus.Success, game, bomb);
        }
    }

    public ExplodeBombResult ExplodeBomb(Guid gameId, Guid bombId)
    {
        if (!Games.TryGetValue(gameId, out var game))
        {
            return new ExplodeBombResult(false);
        }

        lock (game)
        {
            var firstBomb = game.Bombs.FirstOrDefault(item => item.Id == bombId);
            if (firstBomb is null)
            {
                return new ExplodeBombResult(false, game);
            }

            var pending = new Queue<GameBomb>();
            var explodedIds = new HashSet<Guid>();
            var affectedCells = new HashSet<(int X, int Y)>();
            pending.Enqueue(firstBomb);

            while (pending.TryDequeue(out var bomb))
            {
                if (!explodedIds.Add(bomb.Id)) continue;

                foreach (var cell in ExplosionCells(game.Map, bomb))
                {
                    affectedCells.Add(cell);
                    foreach (var chainedBomb in game.Bombs.Where(item =>
                                 item.X == cell.X && item.Y == cell.Y && !explodedIds.Contains(item.Id)))
                    {
                        pending.Enqueue(chainedBomb);
                    }
                }
            }

            game.Bombs.RemoveAll(item => explodedIds.Contains(item.Id));
            foreach (var player in game.Players.Where(item =>
                         item.IsAlive && affectedCells.Contains((item.X, item.Y))))
            {
                player.Lives = Math.Max(0, player.Lives - 1);
                player.IsAlive = player.Lives > 0;
            }

            var explosion = new GameExplosion
            {
                EndsAtUtc = DateTimeOffset.UtcNow.Add(ExplosionDuration),
                Cells = affectedCells
                    .Select(cell => new ExplosionCell { X = cell.X, Y = cell.Y })
                    .ToList()
            };
            game.Explosions.Add(explosion);

            if (game.Players.Count(item => item.IsAlive) <= 1)
            {
                game.Status = GameSessionStatus.Finished;
                game.WinnerPlayerId = game.Players.FirstOrDefault(item => item.IsAlive)?.PlayerId;
            }

            return new ExplodeBombResult(true, game, explosion.Id);
        }
    }

    public GameSession? RemoveExplosion(Guid gameId, Guid explosionId)
    {
        if (!Games.TryGetValue(gameId, out var game)) return null;
        lock (game)
        {
            return game.Explosions.RemoveAll(item => item.Id == explosionId) > 0 ? game : null;
        }
    }

    private static IEnumerable<(int X, int Y)> ExplosionCells(GameMap map, GameBomb bomb)
    {
        yield return (bomb.X, bomb.Y);
        foreach (var (deltaX, deltaY) in new[] { (0, -1), (0, 1), (-1, 0), (1, 0) })
        {
            for (var distance = 1; distance <= bomb.Range; distance += 1)
            {
                var x = bomb.X + deltaX * distance;
                var y = bomb.Y + deltaY * distance;
                if (!map.IsInside(x, y) || !map.IsFree(x, y)) break;
                yield return (x, y);
            }
        }
    }
}
