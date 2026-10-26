namespace Bomberman.Api.DTO;

public sealed class BombResponseDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int PosicionX { get; set; }
    public int PosicionY { get; set; }
    public int Alcance { get; set; }
}
