using Bomberman.Api.Data;
using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Bomberman.Api.Services;

public sealed class GameRoomService(BombermanDbContext dbContext)
{
    private const string CodeCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

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
            Code = await CreateUniqueCodeAsync(cancellationToken),
            HostPlayerId = player.Id,
            Name = name.Trim(),
            MaxPlayers = maxPlayers,
            Status = GameRoomStatus.Waiting,
            Participants =
            [
                new RoomPlayer
                {
                    PlayerId = player.Id,
                    Player = player,
                    IsReady = true
                }
            ]
        };

        dbContext.GameRooms.Add(room);
        await dbContext.SaveChangesAsync(cancellationToken);
        return room;
    }

    public async Task<JoinGameRoomResult> JoinByCodeAsync(
        string code,
        int playerId,
        CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var roomId = await dbContext.GameRooms
            .Where(room => room.Code == normalizedCode)
            .Select(room => (int?)room.Id)
            .SingleOrDefaultAsync(cancellationToken);

        return roomId is null
            ? new JoinGameRoomResult(JoinGameRoomStatus.RoomNotFound)
            : await JoinAsync(roomId.Value, playerId, cancellationToken);
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

    public async Task<SetRoomReadyResult> SetReadyAsync(
        int roomId,
        int playerId,
        bool isReady,
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
            return new SetRoomReadyResult(SetRoomReadyStatus.RoomNotFound);
        }

        if (room.Status != GameRoomStatus.Waiting)
        {
            return new SetRoomReadyResult(SetRoomReadyStatus.RoomNotWaiting);
        }

        await dbContext.Entry(room)
            .Collection(item => item.Participants)
            .Query()
            .Include(item => item.Player)
            .LoadAsync(cancellationToken);

        var participant = room.Participants.SingleOrDefault(item => item.PlayerId == playerId);
        if (participant is null)
        {
            return new SetRoomReadyResult(SetRoomReadyStatus.PlayerNotInRoom);
        }

        if (room.HostPlayerId == playerId)
        {
            return new SetRoomReadyResult(SetRoomReadyStatus.HostCannotChangeReady);
        }

        participant.IsReady = isReady;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new SetRoomReadyResult(SetRoomReadyStatus.Success, room);
    }

    private async Task<string> CreateUniqueCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var code = string.Create(8, CodeCharacters, static (span, characters) =>
            {
                for (var index = 0; index < span.Length; index++)
                {
                    span[index] = characters[RandomNumberGenerator.GetInt32(characters.Length)];
                }
            });

            if (!await dbContext.GameRooms.AnyAsync(room => room.Code == code, cancellationToken))
            {
                return code;
            }
        }

        throw new InvalidOperationException("No se pudo generar un código único para la sala.");
    }
}
