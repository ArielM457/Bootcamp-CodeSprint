using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bomberman.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueRoomCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "game_rooms",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE game_rooms
                SET code = UPPER(LPAD(TO_HEX(id), 8, '0'));
                """);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "game_rooms",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_rooms_code",
                table: "game_rooms",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_game_rooms_code",
                table: "game_rooms");

            migrationBuilder.DropColumn(
                name: "code",
                table: "game_rooms");
        }
    }
}
