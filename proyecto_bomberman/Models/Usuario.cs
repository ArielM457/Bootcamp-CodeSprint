namespace Bomberman.Api.Models;

public sealed class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public Player Player { get; set; } = null!;
}
