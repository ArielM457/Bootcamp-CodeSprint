namespace Bomberman.Api.DTO;

public sealed class CreateBombRequestDto
{
    public int PlayerId { get; set; }
    public int PosicionX { get; set; }
    public int PosicionY { get; set; }
    public int Alcance { get; set; }
}
