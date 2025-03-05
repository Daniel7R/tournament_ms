using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class fixournaments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_prize",
                table: "tournaments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_prize",
                table: "tournaments",
                type: "integer",
                nullable: true);
        }
    }
}
