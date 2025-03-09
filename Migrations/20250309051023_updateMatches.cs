using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class updateMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_teams_TeamWinnerId",
                table: "matches");

            migrationBuilder.DropIndex(
                name: "IX_matches_TeamWinnerId",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "TeamWinnerId",
                table: "matches");

            migrationBuilder.CreateIndex(
                name: "IX_matches_IdTeamWinner",
                table: "matches",
                column: "IdTeamWinner");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_teams_IdTeamWinner",
                table: "matches",
                column: "IdTeamWinner",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_teams_IdTeamWinner",
                table: "matches");

            migrationBuilder.DropIndex(
                name: "IX_matches_IdTeamWinner",
                table: "matches");

            migrationBuilder.AddColumn<int>(
                name: "TeamWinnerId",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_matches_TeamWinnerId",
                table: "matches",
                column: "TeamWinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_teams_TeamWinnerId",
                table: "matches",
                column: "TeamWinnerId",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
