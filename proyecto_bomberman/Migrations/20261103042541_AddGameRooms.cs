using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bomberman.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGameRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_rooms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    max_players = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Waiting")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_rooms", x => x.id);
                    table.CheckConstraint("CK_game_rooms_max_players", "max_players >= 2 AND max_players <= 8");
                });

            migrationBuilder.CreateTable(
                name: "room_players",
                columns: table => new
                {
                    game_room_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_players", x => new { x.game_room_id, x.player_id });
                    table.ForeignKey(
                        name: "FK_room_players_game_rooms_game_room_id",
                        column: x => x.game_room_id,
                        principalTable: "game_rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_room_players_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_room_players_player_id",
                table: "room_players",
                column: "player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_players");

            migrationBuilder.DropTable(
                name: "game_rooms");
        }
    }
}
