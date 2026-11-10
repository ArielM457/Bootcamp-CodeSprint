namespace Bomberman.Api.DTO;

public sealed class GameRoomResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int HostPlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxPlayers { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? GameId { get; set; }
    public IReadOnlyList<RoomPlayerResponseDto> Participants { get; set; } = [];
}
