namespace Bomberman.Api.DTO;

public sealed class GameSessionResponseDto
{
    public Guid Id { get; set; }
    public int RoomId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? WinnerPlayerId { get; set; }
    public int[][] Map { get; set; } = [];
    public IReadOnlyList<PlayerPositionResponseDto> Players { get; set; } = [];
    public IReadOnlyList<GameBombResponseDto> Bombs { get; set; } = [];
    public IReadOnlyList<GameExplosionResponseDto> Explosions { get; set; } = [];
}

public sealed class PlayerPositionResponseDto
{
    public int PlayerId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Lives { get; set; }
    public bool IsAlive { get; set; }
}

public sealed class GameBombResponseDto
{
    public Guid Id { get; set; }
    public int OwnerPlayerId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Range { get; set; }
    public DateTimeOffset PlacedAtUtc { get; set; }
    public DateTimeOffset ExplodesAtUtc { get; set; }
}

public sealed class GameExplosionResponseDto
{
    public Guid Id { get; set; }
    public DateTimeOffset EndsAtUtc { get; set; }
    public IReadOnlyList<ExplosionCellResponseDto> Cells { get; set; } = [];
}

public sealed class ExplosionCellResponseDto
{
    public int X { get; set; }
    public int Y { get; set; }
}
