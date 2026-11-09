namespace Bomberman.Api.Enums;
using Bomberman.Api.Models;

public enum StartGameStatus
{
    Success,
    RoomNotFound,
    PlayerNotInRoom,
    RoomNotWaiting
}

public enum MovePlayerStatus
{
    Success,
    GameNotFound,
    PlayerNotInGame,
    GameFinished,
    InvalidDirection,
    OutsideMap,
    WallBlocked,
    PlayerInAnotherPlayersPosition
}

public sealed record StartGameResult(
    StartGameStatus Status,
    GameSession? Game = null);

public sealed record MovePlayerResult(
    MovePlayerStatus Status,
    GameSession? Game = null);
