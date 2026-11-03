using Bomberman.Api.Models;

namespace Bomberman.Api.Services;

public enum JoinGameRoomStatus
{
    Success,
    RoomNotFound,
    PlayerNotFound,
    AlreadyJoined,
    RoomFull,
    RoomNotWaiting
}

public sealed record JoinGameRoomResult(
    JoinGameRoomStatus Status,
    GameRoom? Room = null);
