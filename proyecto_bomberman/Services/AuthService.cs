using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bomberman.Api.Data;
using Bomberman.Api.DTO;
using Bomberman.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Bomberman.Api.Services;

public sealed class AuthService(
    BombermanDbContext dbContext,
    UsuarioService usuarioService,
    IConfiguration configuration)
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public async Task<AuthResponseDto?> RegisterAsync(
        CreateUsuarioRequestDto request,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioService.CreateAsync(
            request.Nombre,
            request.Correo,
            request.Password,
            cancellationToken);

        return usuario is null ? null : CreateAuthResponse(usuario);
    }

    public async Task<AuthResponseDto?> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var correoNormalizado = request.Correo.Trim().ToLowerInvariant();
        var usuario = await dbContext.Usuarios
            .AsNoTracking()
            .Include(item => item.Player)
            .FirstOrDefaultAsync(
                item => item.Correo == correoNormalizado,
                cancellationToken);

        if (usuario is null || usuario.Player is null)
        {
            return null;
        }

        PasswordVerificationResult resultado;
        try
        {
            resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                request.Password);
        }
        catch (FormatException)
        {
            return null;
        }

        if (resultado == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return CreateAuthResponse(usuario);
    }

    private AuthResponseDto CreateAuthResponse(Usuario usuario)
    {
        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Falta configurar Jwt:Issuer.");
        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Falta configurar Jwt:Audience.");
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Falta configurar Jwt:Key.");
        var expirationMinutes = configuration.GetValue<int?>("Jwt:ExpirationMinutes") ?? 120;
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Correo),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nombre),
            new("player_id", usuario.Player.Id.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc,
            Usuario = new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                PlayerId = usuario.Player.Id
            }
        };
    }
}
