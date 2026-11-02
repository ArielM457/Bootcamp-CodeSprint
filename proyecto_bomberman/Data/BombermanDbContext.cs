using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bomberman.Api.Data;

public sealed class BombermanDbContext(DbContextOptions<BombermanDbContext> options)
    : DbContext(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BombermanDbContext).Assembly);
    }
}
