using Bomberman.Api.Data;
using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bomberman.Api.Services;

public sealed class PlayerService(BombermanDbContext dbContext)
{
    public Task<List<Player>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext.Players
            .AsNoTracking()
            .OrderBy(player => player.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(player => player.Id == id, cancellationToken);
    }

    public async Task<Player> CreateAsync(string nombre, CancellationToken cancellationToken)
    {
        var player = new Player
        {
            Nombre = nombre
        };

        dbContext.Players.Add(player);
        await dbContext.SaveChangesAsync(cancellationToken);
        return player;
    }

    public async Task<Player?> ReplaceAsync(
        int id,
        string nombre,
        int vida,
        double puntaje,
        CancellationToken cancellationToken)
    {
        var player = await dbContext.Players.FindAsync([id], cancellationToken);
        if (player is null)
        {
            return null;
        }

        player.Nombre = nombre;
        player.vida = vida;
        player.puntaje = puntaje;
        await dbContext.SaveChangesAsync(cancellationToken);
        return player;
    }

    public async Task<Player?> UpdateAsync(
        int id,
        string? nombre,
        int? vida,
        double? puntaje,
        CancellationToken cancellationToken)
    {
        var player = await dbContext.Players.FindAsync([id], cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return player;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var player = await dbContext.Players.FindAsync([id], cancellationToken);       dbContext.Players.Remove(player);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
