using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class updateTeamsMatches2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_teams_id_team_winner",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "FK_teams_matches_matches_MatchId",
                table: "teams_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_teams_matches_teams_TeamId",
                table: "teams_matches");

            migrationBuilder.DropIndex(
                name: "IX_teams_matches_MatchId",
                table: "teams_matches");

            migrationBuilder.DropIndex(
                name: "IX_teams_matches_TeamId",
                table: "teams_matches");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "teams_matches");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "teams_matches");

            migrationBuilder.CreateIndex(
                name: "IX_teams_matches_id_match",
                table: "teams_matches",
                column: "id_match");

            migrationBuilder.CreateIndex(
                name: "IX_teams_matches_id_team",
                table: "teams_matches",
                column: "id_team");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_teams_id_team_winner",
                table: "matches",
                column: "id_team_winner",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_matches_matches_id_match",
                table: "teams_matches",
                column: "id_match",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_matches_teams_id_team",
                table: "teams_matches",
                column: "id_team",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_teams_id_team_winner",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "FK_teams_matches_matches_id_match",
                table: "teams_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_teams_matches_teams_id_team",
                table: "teams_matches");

            migrationBuilder.DropIndex(
                name: "IX_teams_matches_id_match",
                table: "teams_matches");

            migrationBuilder.DropIndex(
                name: "IX_teams_matches_id_team",
                table: "teams_matches");

            migrationBuilder.AddColumn<int>(
                name: "MatchId",
                table: "teams_matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "teams_matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_teams_matches_MatchId",
                table: "teams_matches",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_teams_matches_TeamId",
                table: "teams_matches",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_teams_id_team_winner",
                table: "matches",
                column: "id_team_winner",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_matches_matches_MatchId",
                table: "teams_matches",
                column: "MatchId",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_matches_teams_TeamId",
                table: "teams_matches",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
