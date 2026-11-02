namespace Bomberman.Api.DTO;

public sealed class UsuarioResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public int PlayerId { get; set; }
}
