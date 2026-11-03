using Bomberman.Api.Data;
using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;

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
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken);

        var room = await dbContext.GameRooms
            .FromSqlInterpolated(
                $"SELECT * FROM game_rooms WHERE id = {roomId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

        if (room is null)
        {
            return new JoinGameRoomResult(JoinGameRoomStatus.RoomNotFound);
        }

        await dbContext.Entry(room)
            .Collection(item => item.Participants)
            .Query()
            .Include(item => item.Player)
            .LoadAsync(cancellationToken);

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
}
