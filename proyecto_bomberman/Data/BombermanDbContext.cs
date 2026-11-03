using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bomberman.Api.Data;

public sealed class BombermanDbContext(DbContextOptions<BombermanDbContext> options)
    : DbContext(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<GameRoom> GameRooms => Set<GameRoom>();
    public DbSet<RoomPlayer> RoomPlayers => Set<RoomPlayer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BombermanDbContext).Assembly);
    }
}
