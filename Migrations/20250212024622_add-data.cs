using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class adddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "alias", "code", "name" },
                values: new object[] { 1, "Racing", "0235", "Carreras" });

            migrationBuilder.InsertData(
                table: "games",
                columns: new[] { "id", "name", "players" },
                values: new object[] { 1, "Need For Speed", 10 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "games",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
