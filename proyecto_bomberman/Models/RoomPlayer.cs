namespace Bomberman.Api.Models;

public sealed class RoomPlayer
{
    public int GameRoomId { get; set; }
    public GameRoom GameRoom { get; set; } = null!;

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
}
