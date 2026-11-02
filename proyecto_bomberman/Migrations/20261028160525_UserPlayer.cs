using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bomberman.Api.Migrations
{
    /// <inheritdoc />
    public partial class UserPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "id", "correo", "nombre", "password_hash" },
                values: new object[,]
                {
                    { 1, "usuario1@example.com", "Usuario 1", "hashed_password_1" },
                    { 2, "usuario2@example.com", "Usuario 2", "hashed_password_2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "usuarios",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "usuarios",
                keyColumn: "id",
                keyValue: 2);
        }
    }
}
