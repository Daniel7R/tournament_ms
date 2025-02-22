using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class adddata2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 1,
                column: "LimitParticipant",
                value: 20);

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "alias", "code", "LimitParticipant", "name" },
                values: new object[] { 2, "Strategy", "0236", 60, "Estrategia" });

            migrationBuilder.UpdateData(
                table: "games",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "MaxPlayersPerTeam", "MaxTeams" },
                values: new object[] { 10, 10 });

            migrationBuilder.InsertData(
                table: "games",
                columns: new[] { "id", "IsCooperative", "MaxPlayersPerTeam", "MaxTeams", "name", "players" },
                values: new object[] { 2, true, 5, 2, "League Of Legends", 10 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "games",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 1,
                column: "LimitParticipant",
                value: null);

            migrationBuilder.UpdateData(
                table: "games",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "MaxPlayersPerTeam", "MaxTeams" },
                values: new object[] { 0, 0 });
        }
    }
}
