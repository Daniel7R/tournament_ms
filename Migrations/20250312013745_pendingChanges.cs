using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class pendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_stream",
                table: "matches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_stream",
                table: "matches",
                type: "integer",
                nullable: true);
        }
    }
}
