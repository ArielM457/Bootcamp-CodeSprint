namespace Bomberman.Api.Models;

public sealed class GameRoom
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxPlayers { get; set; }
    public GameRoomStatus Status { get; set; } = GameRoomStatus.Waiting;
    public ICollection<RoomPlayer> Participants { get; set; } = [];
}
