using Bomberman.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bomberman.Api.Data.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(usuario => usuario.Id);

        builder.Property(usuario => usuario.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(usuario => usuario.Nombre)
            .HasColumnName("nombre")
            .IsRequired();

        builder.Property(usuario => usuario.Correo)
            .HasColumnName("correo")
            .IsRequired();

        builder.Property(usuario => usuario.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.HasIndex(usuario => usuario.Correo)
            .IsUnique();

        builder.HasOne(usuario => usuario.Player)
            .WithOne(player => player.Usuario)
            .HasForeignKey<Player>(player => player.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
