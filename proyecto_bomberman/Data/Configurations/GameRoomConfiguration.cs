using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bomberman.Api.Data.Configurations;

public sealed class GameRoomConfiguration : IEntityTypeConfiguration<GameRoom>
{
    public void Configure(EntityTypeBuilder<GameRoom> builder)
    {
        builder.ToTable("game_rooms", table =>
            table.HasCheckConstraint(
                "CK_game_rooms_max_players",
                "max_players >= 2 AND max_players <= 8"));

        builder.HasKey(room => room.Id);

        builder.Property(room => room.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(room => room.Name)
            .HasColumnName("name")
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(room => room.MaxPlayers)
            .HasColumnName("max_players")
            .IsRequired();

        builder.Property(room => room.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(GameRoomStatus.Waiting)
            .IsRequired();
    }
}
