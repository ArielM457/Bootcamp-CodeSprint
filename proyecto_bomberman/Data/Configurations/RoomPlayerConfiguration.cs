using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bomberman.Api.Data.Configurations;

public sealed class RoomPlayerConfiguration : IEntityTypeConfiguration<RoomPlayer>
{
    public void Configure(EntityTypeBuilder<RoomPlayer> builder)
    {
        builder.ToTable("room_players");

        builder.HasKey(item => new { item.GameRoomId, item.PlayerId });

        builder.Property(item => item.GameRoomId)
            .HasColumnName("game_room_id");

        builder.Property(item => item.PlayerId)
            .HasColumnName("player_id");

        builder.HasOne(item => item.GameRoom)
            .WithMany(room => room.Participants)
            .HasForeignKey(item => item.GameRoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Player)
            .WithMany(player => player.Rooms)
            .HasForeignKey(item => item.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
