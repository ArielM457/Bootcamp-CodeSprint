namespace Bomberman.Api.Models;

public sealed class GameBomb
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int OwnerPlayerId { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public int Range { get; init; } = 2;
    public DateTimeOffset PlacedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExplodesAtUtc { get; init; }
}

public sealed class GameExplosion
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset EndsAtUtc { get; init; }
    public List<ExplosionCell> Cells { get; init; } = [];
}

public sealed class ExplosionCell
{
    public int X { get; init; }
    public int Y { get; init; }
}
