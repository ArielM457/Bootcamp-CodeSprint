using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bomberman.Api.Data.Configurations;

public sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("players");

        builder.HasKey(player => player.Id);

        builder.Property(player => player.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(player => player.Nombre)
            .HasColumnName("nombre")
            .IsRequired();

        builder.Property(player => player.vida)
            .HasColumnName("vida")
            .HasDefaultValue(3);

        builder.Property(player => player.puntaje)
            .HasColumnName("puntaje")
            .HasDefaultValue(0.0);

        builder.HasData(
            new Player { Id = 1, Nombre = "Jugador 1", vida = 3, puntaje = 0.0 },
            new Player { Id = 2, Nombre = "Jugador 2", vida = 3, puntaje = 0.0 });
    }
}
