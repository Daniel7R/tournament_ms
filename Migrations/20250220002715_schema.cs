using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "tournaments");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "tournaments");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "tournaments",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "tournaments",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "tournaments",
                newName: "tournament_status");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "tournaments",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "tournaments",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "is_paid",
                table: "tournaments",
                newName: "is_free");

            migrationBuilder.RenameColumn(
                name: "MaxPlayers",
                table: "tournaments",
                newName: "id_prize");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "tournaments",
                newName: "id_organizer");

            migrationBuilder.AddColumn<int>(
                name: "id_team_winner_tournmanent",
                table: "tournaments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCooperative",
                table: "games",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxPlayersPerTeam",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxTeams",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LimitParticipant",
                table: "categories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Prize",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IdTournament = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Total = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prize", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prize_tournaments_Id",
                        column: x => x.Id,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tournaments_users_roles",
                columns: table => new
                {
                    id_tournament = table.Column<int>(type: "integer", nullable: false),
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    id_role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournaments_users_roles", x => new { x.id_user, x.id_tournament, x.id_role });
                    table.ForeignKey(
                        name: "FK_tournaments_users_roles_tournaments_id_tournament",
                        column: x => x.id_tournament,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                columns: new[] { "IsCooperative", "MaxPlayersPerTeam", "MaxTeams" },
                values: new object[] { false, 0, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_users_roles_id_tournament",
                table: "tournaments_users_roles",
                column: "id_tournament");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prize");

            migrationBuilder.DropTable(
                name: "tournaments_users_roles");

            migrationBuilder.DropColumn(
                name: "id_team_winner_tournmanent",
                table: "tournaments");

            migrationBuilder.DropColumn(
                name: "IsCooperative",
                table: "games");

            migrationBuilder.DropColumn(
                name: "MaxPlayersPerTeam",
                table: "games");

            migrationBuilder.DropColumn(
                name: "MaxTeams",
                table: "games");

            migrationBuilder.DropColumn(
                name: "LimitParticipant",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "tournaments",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "tournaments",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "tournament_status",
                table: "tournaments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "tournaments",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "tournaments",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "is_free",
                table: "tournaments",
                newName: "is_paid");

            migrationBuilder.RenameColumn(
                name: "id_prize",
                table: "tournaments",
                newName: "MaxPlayers");

            migrationBuilder.RenameColumn(
                name: "id_organizer",
                table: "tournaments",
                newName: "CreatedBy");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "tournaments",
                type: "numeric(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "tournaments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
