namespace Bomberman.Api.Models;

public sealed class Player
{   
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<RoomPlayer> Rooms { get; set; } = [];
    
    public int vida { get; set; } = 3;
    public double puntaje { get; set; } = 0.0;
}
