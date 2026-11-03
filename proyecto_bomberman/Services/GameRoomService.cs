using System.Data;
using Bomberman.Api.Data;
using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Bomberman.Api.Services;

public sealed class GameRoomService(BombermanDbContext dbContext)
{
    public Task<List<GameRoom>> GetAvailableAsync(CancellationToken cancellationToken)
    {
        return dbContext.GameRooms
            .AsNoTracking()
            .Include(room => room.Participants)
                .ThenInclude(item => item.Player)
            .Where(room =>
                room.Status == GameRoomStatus.Waiting &&
                room.Participants.Count < room.MaxPlayers)
            .OrderBy(room => room.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<GameRoom?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.GameRooms
            .AsNoTracking()
            .Include(room => room.Participants)
                .ThenInclude(item => item.Player)
            .FirstOrDefaultAsync(room => room.Id == id, cancellationToken);
    }

    public async Task<GameRoom?> CreateAsync(
        string name,
        int maxPlayers,
        int playerId,
        CancellationToken cancellationToken)
    {
        var player = await dbContext.Players.FindAsync([playerId], cancellationToken);
        if (player is null)
        {
            return null;
        }

        var room = new GameRoom
        {
            Name = name.Trim(),
            MaxPlayers = maxPlayers,
            Status = GameRoomStatus.Waiting,
            Participants =
            [
                new RoomPlayer
                {
                    PlayerId = player.Id,
                    Player = player
                }
            ]
        };

        dbContext.GameRooms.Add(room);
        await dbContext.SaveChangesAsync(cancellationToken);
        return room;
    }

    public async Task<JoinGameRoomResult> JoinAsync(
        int roomId,
        int playerId,
        CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            dbContext.ChangeTracker.Clear();
            await using var transaction = await dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

            try
            {
                var room = await dbContext.GameRooms
                    .Include(item => item.Participants)
                        .ThenInclude(item => item.Player)
                    .FirstOrDefaultAsync(item => item.Id == roomId, cancellationToken);

                if (room is null)
                {
                    return new JoinGameRoomResult(JoinGameRoomStatus.RoomNotFound);
                }

                if (room.Participants.Any(item => item.PlayerId == playerId))
                {
                    return new JoinGameRoomResult(JoinGameRoomStatus.AlreadyJoined);
                }

                if (room.Status != GameRoomStatus.Waiting)
                {
                    return new JoinGameRoomResult(JoinGameRoomStatus.RoomNotWaiting);
                }

                if (room.Participants.Count >= room.MaxPlayers)
                {
                    return new JoinGameRoomResult(JoinGameRoomStatus.RoomFull);
                }

                var player = await dbContext.Players.FindAsync([playerId], cancellationToken);
                if (player is null)
                {
                    return new JoinGameRoomResult(JoinGameRoomStatus.PlayerNotFound);
                }

                room.Participants.Add(new RoomPlayer
                {
                    GameRoomId = room.Id,
                    PlayerId = player.Id,
                    Player = player
                });

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return new JoinGameRoomResult(JoinGameRoomStatus.Success, room);
            }
            catch (PostgresException exception)
                when (exception.SqlState == PostgresErrorCodes.SerializationFailure)
            {
                await transaction.RollbackAsync(cancellationToken);

                if (attempt == maxAttempts)
                {
                    return new JoinGameRoomResult(JoinGameRoomStatus.ConcurrentRequest);
                }
            }
            catch (DbUpdateException exception)
                when (exception.InnerException is PostgresException postgresException &&
                      postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                await transaction.RollbackAsync(cancellationToken);
                return new JoinGameRoomResult(JoinGameRoomStatus.AlreadyJoined);
            }
        }

        return new JoinGameRoomResult(JoinGameRoomStatus.ConcurrentRequest);
    }
}
