using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class updateUnique : Migration
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
                    LimitParticipant = table.Column<int>(type: "integer", nullable: false)
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
                name: "prizes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    total = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prizes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_tournament = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    current_members = table.Column<int>(type: "integer", nullable: false),
                    max_members = table.Column<int>(type: "integer", nullable: false),
                    is_full = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.id);
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
                    id_prize = table.Column<int>(type: "integer", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_tournaments_prizes_id_prize",
                        column: x => x.id_prize,
                        principalTable: "prizes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "teams_members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_team = table.Column<int>(type: "integer", nullable: false),
                    id_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teams_members_teams_id_team",
                        column: x => x.id_team,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_tournament = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    id_team_winner = table.Column<int>(type: "integer", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.id);
                    table.ForeignKey(
                        name: "FK_matches_teams_id_team_winner",
                        column: x => x.id_team_winner,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_matches_tournaments_id_tournament",
                        column: x => x.id_tournament,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teams_matches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_team = table.Column<int>(type: "integer", nullable: false),
                    id_match = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams_matches", x => x.id);
                    table.ForeignKey(
                        name: "FK_teams_matches_matches_id_match",
                        column: x => x.id_match,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teams_matches_teams_id_team",
                        column: x => x.id_team,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tournaments_users_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_tournament = table.Column<int>(type: "integer", nullable: true),
                    id_match = table.Column<int>(type: "integer", nullable: true),
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournaments_users_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_tournaments_users_roles_matches_id_match",
                        column: x => x.id_match,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tournaments_users_roles_tournaments_id_tournament",
                        column: x => x.id_tournament,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "alias", "code", "LimitParticipant", "name" },
                values: new object[,]
                {
                    { 1, "Racing", "0235", 10, "Carreras" },
                    { 2, "Strategy", "0236", 10, "Estrategia" }
                });

            migrationBuilder.InsertData(
                table: "games",
                columns: new[] { "id", "IsCooperative", "MaxPlayersPerTeam", "MaxTeams", "name", "players" },
                values: new object[,]
                {
                    { 1, false, 1, 10, "Need For Speed", 10 },
                    { 2, true, 5, 2, "League Of Legends", 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_alias",
                table: "categories",
                column: "alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_code",
                table: "categories",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_name",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_matches_id_team_winner",
                table: "matches",
                column: "id_team_winner");

            migrationBuilder.CreateIndex(
                name: "IX_matches_id_tournament",
                table: "matches",
                column: "id_tournament");

            migrationBuilder.CreateIndex(
                name: "IX_teams_matches_id_match",
                table: "teams_matches",
                column: "id_match");

            migrationBuilder.CreateIndex(
                name: "IX_teams_matches_id_team",
                table: "teams_matches",
                column: "id_team");

            migrationBuilder.CreateIndex(
                name: "IX_teams_members_id_team",
                table: "teams_members",
                column: "id_team");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_id_category",
                table: "tournaments",
                column: "id_category");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_id_game",
                table: "tournaments",
                column: "id_game");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_id_prize",
                table: "tournaments",
                column: "id_prize",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_users_roles_id_match",
                table: "tournaments_users_roles",
                column: "id_match");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_users_roles_id_tournament",
                table: "tournaments_users_roles",
                column: "id_tournament");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_users_roles_id_user_id_match",
                table: "tournaments_users_roles",
                columns: new[] { "id_user", "id_match" },
                unique: true,
                filter: "id_match IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_users_roles_id_user_id_tournament",
                table: "tournaments_users_roles",
                columns: new[] { "id_user", "id_tournament" },
                unique: true,
                filter: "id_tournament IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropTable(
                name: "prizes");
        }
    }
}
