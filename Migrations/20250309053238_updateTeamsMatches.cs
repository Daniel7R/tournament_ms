using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class updateTeamsMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_teams_IdTeamWinner",
                table: "matches");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "teams_matches",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IdTeam",
                table: "teams_matches",
                newName: "id_team");

            migrationBuilder.RenameColumn(
                name: "IdMatch",
                table: "teams_matches",
                newName: "id_match");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "matches",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "matches",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "IdTeamWinner",
                table: "matches",
                newName: "id_team_winner");

            migrationBuilder.RenameColumn(
                name: "IdStream",
                table: "matches",
                newName: "id_stream");

            migrationBuilder.RenameIndex(
                name: "IX_matches_IdTeamWinner",
                table: "matches",
                newName: "IX_matches_id_team_winner");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_teams_id_team_winner",
                table: "matches",
                column: "id_team_winner",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_teams_id_team_winner",
                table: "matches");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "teams_matches",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id_team",
                table: "teams_matches",
                newName: "IdTeam");

            migrationBuilder.RenameColumn(
                name: "id_match",
                table: "teams_matches",
                newName: "IdMatch");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "matches",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "matches",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "id_team_winner",
                table: "matches",
                newName: "IdTeamWinner");

            migrationBuilder.RenameColumn(
                name: "id_stream",
                table: "matches",
                newName: "IdStream");

            migrationBuilder.RenameIndex(
                name: "IX_matches_id_team_winner",
                table: "matches",
                newName: "IX_matches_IdTeamWinner");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_teams_IdTeamWinner",
                table: "matches",
                column: "IdTeamWinner",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
