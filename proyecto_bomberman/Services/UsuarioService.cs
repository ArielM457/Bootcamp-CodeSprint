using Bomberman.Api.Data;
using Bomberman.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bomberman.Api.Services;

public sealed class UsuarioService(BombermanDbContext dbContext)
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public Task<List<Usuario>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Player)
            .OrderBy(usuario => usuario.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Player)
            .FirstOrDefaultAsync(usuario => usuario.Id == id, cancellationToken);
    }

    public async Task<Usuario?> CreateAsync(
        string nombre,
        string correo,
        string password,
        CancellationToken cancellationToken)
    {
        var correoNormalizado = correo.Trim().ToLowerInvariant();

        var correoExiste = await dbContext.Usuarios
            .AnyAsync(usuario => usuario.Correo == correoNormalizado, cancellationToken);

        if (correoExiste)
        {
            return null;
        }

        var usuario = new Usuario
        {
            Nombre = nombre.Trim(),
            Correo = correoNormalizado,
            Player = new Player
            {
                Nombre = nombre.Trim()
            }
        };

        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, password);

        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);

        return usuario;
    }
}
