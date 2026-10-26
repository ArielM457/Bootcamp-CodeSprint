namespace Bomberman.Api.Models;

public sealed class Bomb
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int PosicionX { get; set; }
    public int PosicionY { get; set; }
    public int Alcance { get; set; }
}
