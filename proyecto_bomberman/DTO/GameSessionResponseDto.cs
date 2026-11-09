namespace Bomberman.Api.DTO;

public sealed class GameSessionResponseDto
{
    public Guid Id { get; set; }
    public int RoomId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int[][] Map { get; set; } = [];
    public IReadOnlyList<PlayerPositionResponseDto> Players { get; set; } = [];
}

public sealed class PlayerPositionResponseDto
{
    public int PlayerId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}
