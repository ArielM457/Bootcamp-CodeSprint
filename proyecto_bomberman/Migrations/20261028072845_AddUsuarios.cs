using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bomberman.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "usuario_id",
                table: "players",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    correo = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "players",
                keyColumn: "id",
                keyValue: 1,
                column: "usuario_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "players",
                keyColumn: "id",
                keyValue: 2,
                column: "usuario_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_players_usuario_id",
                table: "players",
                column: "usuario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_correo",
                table: "usuarios",
                column: "correo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_players_usuarios_usuario_id",
                table: "players",
                column: "usuario_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_players_usuarios_usuario_id",
                table: "players");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_players_usuario_id",
                table: "players");

            migrationBuilder.DropColumn(
                name: "usuario_id",
                table: "players");
        }
    }
}
