using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class updateTeamsMembersFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_teams_members_teams_IdTeam",
                table: "teams_members");

            migrationBuilder.RenameColumn(
                name: "IdUser",
                table: "teams_members",
                newName: "id_user");

            migrationBuilder.RenameColumn(
                name: "IdTeam",
                table: "teams_members",
                newName: "id_team");

            migrationBuilder.RenameIndex(
                name: "IX_teams_members_IdTeam",
                table: "teams_members",
                newName: "IX_teams_members_id_team");

            migrationBuilder.AlterColumn<int>(
                name: "IdTeamWinner",
                table: "matches",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "IdStream",
                table: "matches",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 1,
                column: "LimitParticipant",
                value: 10);

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 2,
                column: "LimitParticipant",
                value: 10);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_members_teams_id_team",
                table: "teams_members",
                column: "id_team",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_teams_members_teams_id_team",
                table: "teams_members");

            migrationBuilder.RenameColumn(
                name: "id_user",
                table: "teams_members",
                newName: "IdUser");

            migrationBuilder.RenameColumn(
                name: "id_team",
                table: "teams_members",
                newName: "IdTeam");

            migrationBuilder.RenameIndex(
                name: "IX_teams_members_id_team",
                table: "teams_members",
                newName: "IX_teams_members_IdTeam");

            migrationBuilder.AlterColumn<int>(
                name: "IdTeamWinner",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdStream",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 1,
                column: "LimitParticipant",
                value: 20);

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 2,
                column: "LimitParticipant",
                value: 60);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_members_teams_IdTeam",
                table: "teams_members",
                column: "IdTeam",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
