using Bomberman.Api.Models;

namespace Bomberman.Api.Services;

public enum SetRoomReadyStatus
{
    Success,
    RoomNotFound,
    PlayerNotInRoom,
    HostCannotChangeReady,
    RoomNotWaiting
}

public sealed record SetRoomReadyResult(
    SetRoomReadyStatus Status,
    GameRoom? Room = null);
