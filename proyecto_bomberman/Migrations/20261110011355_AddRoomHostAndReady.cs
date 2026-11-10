using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bomberman.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomHostAndReady : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_ready",
                table: "room_players",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "host_player_id",
                table: "game_rooms",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE game_rooms AS room
                SET host_player_id = participant.player_id
                FROM (
                    SELECT game_room_id, MIN(player_id) AS player_id
                    FROM room_players
                    GROUP BY game_room_id
                ) AS participant
                WHERE participant.game_room_id = room.id;

                UPDATE room_players AS participant
                SET is_ready = TRUE
                FROM game_rooms AS room
                WHERE participant.game_room_id = room.id
                  AND participant.player_id = room.host_player_id;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "host_player_id",
                table: "game_rooms",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_rooms_host_player_id",
                table: "game_rooms",
                column: "host_player_id");

            migrationBuilder.AddForeignKey(
                name: "FK_game_rooms_players_host_player_id",
                table: "game_rooms",
                column: "host_player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_rooms_players_host_player_id",
                table: "game_rooms");

            migrationBuilder.DropIndex(
                name: "IX_game_rooms_host_player_id",
                table: "game_rooms");

            migrationBuilder.DropColumn(
                name: "is_ready",
                table: "room_players");

            migrationBuilder.DropColumn(
                name: "host_player_id",
                table: "game_rooms");
        }
    }
}
