using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class modifiedTournaments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prizes_tournaments_id",
                table: "prizes");

            migrationBuilder.AlterColumn<int>(
                name: "id_prize",
                table: "tournaments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "prizes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prizes_tournaments_id_tournament",
                table: "prizes");

            migrationBuilder.DropIndex(
                name: "IX_prizes_id_tournament",
                table: "prizes");

            migrationBuilder.AlterColumn<int>(
                name: "id_prize",
                table: "tournaments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "prizes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_prizes_tournaments_id",
                table: "prizes",
                column: "id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
