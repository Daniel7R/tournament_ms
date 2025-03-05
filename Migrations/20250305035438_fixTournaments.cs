using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class fixTournaments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prizes_tournaments_id_tournament",
                table: "prizes");

            migrationBuilder.DropIndex(
                name: "IX_prizes_id_tournament",
                table: "prizes");

            migrationBuilder.DropColumn(
                name: "id_tournament",
                table: "prizes");

            migrationBuilder.AddColumn<int>(
                name: "id_prize",
                table: "tournaments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_id_prize",
                table: "tournaments",
                column: "id_prize",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tournaments_prizes_id_prize",
                table: "tournaments",
                column: "id_prize",
                principalTable: "prizes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tournaments_prizes_id_prize",
                table: "tournaments");

            migrationBuilder.DropIndex(
                name: "IX_tournaments_id_prize",
                table: "tournaments");

            migrationBuilder.DropColumn(
                name: "id_prize",
                table: "tournaments");

            migrationBuilder.AddColumn<int>(
                name: "id_tournament",
                table: "prizes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_prizes_id_tournament",
                table: "prizes",
                column: "id_tournament",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_prizes_tournaments_id_tournament",
                table: "prizes",
                column: "id_tournament",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
