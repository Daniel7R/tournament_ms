using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    alias = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LimitParticipant = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    players = table.Column<int>(type: "integer", nullable: false),
                    IsCooperative = table.Column<bool>(type: "boolean", nullable: false),
                    MaxTeams = table.Column<int>(type: "integer", nullable: false),
                    MaxPlayersPerTeam = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTournament = table.Column<int>(type: "integer", nullable: false),
                    IdGame = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsFull = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tournaments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    id_category = table.Column<int>(type: "integer", nullable: false),
                    id_game = table.Column<int>(type: "integer", nullable: false),
                    is_free = table.Column<bool>(type: "boolean", nullable: false),
                    id_organizer = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    tournament_status = table.Column<string>(type: "text", nullable: false),
                    id_team_winner_tournmanent = table.Column<int>(type: "integer", nullable: true),
                    id_prize = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournaments", x => x.id);
                    table.ForeignKey(
                        name: "FK_tournaments_categories_id_category",
                        column: x => x.id_category,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tournaments_games_id_game",
                        column: x => x.id_game,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teams_members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTeam = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    IdUser = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teams_members_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_tournament = table.Column<int>(type: "integer", nullable: false),
                    IdStream = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IdTeamWinner = table.Column<int>(type: "integer", nullable: false),
                    TeamWinnerId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.id);
                    table.ForeignKey(
                        name: "FK_matches_teams_TeamWinnerId",
                        column: x => x.TeamWinnerId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_matches_tournaments_id_tournament",
                        column: x => x.id_tournament,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IdTournament = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Total = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prizes_tournaments_Id",
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

            migrationBuilder.CreateTable(
                name: "teams_matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTeam = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    IdMatch = table.Column<int>(type: "integer", nullable: false),
                    MatchId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams_matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teams_matches_matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teams_matches_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "alias", "code", "LimitParticipant", "name" },
                values: new object[,]
                {
                    { 1, "Racing", "0235", 20, "Carreras" },
                    { 2, "Strategy", "0236", 60, "Estrategia" }
                });

            migrationBuilder.InsertData(
                table: "games",
                columns: new[] { "id", "IsCooperative", "MaxPlayersPerTeam", "MaxTeams", "name", "players" },
                values: new object[,]
                {
                    { 1, false, 10, 10, "Need For Speed", 10 },
                    { 2, true, 5, 2, "League Of Legends", 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_matches_id_tournament",
                table: "matches",
                column: "id_tournament");

            migrationBuilder.CreateIndex(
                name: "IX_matches_TeamWinnerId",
                table: "matches",
                column: "TeamWinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_teams_matches_MatchId",
                table: "teams_matches",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_teams_matches_TeamId",
                table: "teams_matches",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_teams_members_TeamId",
                table: "teams_members",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_id_category",
                table: "tournaments",
                column: "id_category");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_id_game",
                table: "tournaments",
                column: "id_game");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_users_roles_id_tournament",
                table: "tournaments_users_roles",
                column: "id_tournament");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prizes");

            migrationBuilder.DropTable(
                name: "teams_matches");

            migrationBuilder.DropTable(
                name: "teams_members");

            migrationBuilder.DropTable(
                name: "tournaments_users_roles");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "tournaments");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "games");
        }
    }
}
