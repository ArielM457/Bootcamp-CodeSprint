namespace Bomberman.Api.Enums;
using Bomberman.Api.Models;

public enum StartGameStatus
{
    Success,
    RoomNotFound,
    PlayerNotInRoom,
    PlayerNotHost,
    NotEnoughPlayers,
    PlayersNotReady,
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
    BombBlocked,
    MoveTooSoon,
    PlayerEliminated,
    PlayerInAnotherPlayersPosition
}

public enum PlaceBombStatus
{
    Success,
    GameNotFound,
    PlayerNotInGame,
    PlayerEliminated,
    GameFinished,
    PlayerAlreadyHasBomb,
    CellAlreadyHasBomb
}

public sealed record StartGameResult(
    StartGameStatus Status,
    GameSession? Game = null);

public sealed record MovePlayerResult(
    MovePlayerStatus Status,
    GameSession? Game = null);

public sealed record PlaceBombResult(
    PlaceBombStatus Status,
    GameSession? Game = null,
    GameBomb? Bomb = null);

public sealed record ExplodeBombResult(
    bool Updated,
    GameSession? Game = null,
    Guid? ExplosionId = null);
