namespace Bomberman.Api.DTO;

public sealed class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public UsuarioResponseDto Usuario { get; set; } = null!;
}
