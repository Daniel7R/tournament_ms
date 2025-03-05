using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentMS.Migrations
{
    /// <inheritdoc />
    public partial class modifiedUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prizes_tournaments_Id",
                table: "prizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tournaments_users_roles",
                table: "tournaments_users_roles");

            migrationBuilder.DropColumn(
                name: "id_role",
                table: "tournaments_users_roles");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "prizes",
                newName: "total");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "prizes",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "prizes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IdTournament",
                table: "prizes",
                newName: "id_tournament");

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "tournaments_users_roles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IdMatch",
                table: "tournaments_users_roles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tournaments_users_roles",
                table: "tournaments_users_roles",
                columns: new[] { "id_user", "id_tournament", "role" });

            migrationBuilder.AddForeignKey(
                name: "FK_prizes_tournaments_id",
                table: "prizes",
                column: "id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prizes_tournaments_id",
                table: "prizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tournaments_users_roles",
                table: "tournaments_users_roles");

            migrationBuilder.DropColumn(
                name: "role",
                table: "tournaments_users_roles");

            migrationBuilder.DropColumn(
                name: "IdMatch",
                table: "tournaments_users_roles");

            migrationBuilder.RenameColumn(
                name: "total",
                table: "prizes",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "prizes",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "prizes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id_tournament",
                table: "prizes",
                newName: "IdTournament");

            migrationBuilder.AddColumn<int>(
                name: "id_role",
                table: "tournaments_users_roles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tournaments_users_roles",
                table: "tournaments_users_roles",
                columns: new[] { "id_user", "id_tournament", "id_role" });

            migrationBuilder.AddForeignKey(
                name: "FK_prizes_tournaments_Id",
                table: "prizes",
                column: "Id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
