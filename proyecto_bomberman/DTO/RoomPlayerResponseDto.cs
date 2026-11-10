namespace Bomberman.Api.DTO;

public sealed class RoomPlayerResponseDto
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsReady { get; set; }
}
