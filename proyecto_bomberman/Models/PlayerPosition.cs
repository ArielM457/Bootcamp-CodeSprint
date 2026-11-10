namespace Bomberman.Api.Models;

public sealed class PlayerPosition
{
    public const int InitialLives = 3;

    public int PlayerId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Lives { get; set; } = InitialLives;
    public bool IsAlive { get; set; } = true;
    public DateTimeOffset LastMovedAtUtc { get; set; } = DateTimeOffset.MinValue;
}
