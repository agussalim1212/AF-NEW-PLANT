using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkeletonApi.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class part10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AspNetUsers_id_user",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "id_user",
                table: "Accounts",
                newName: "username");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_id_user",
                table: "Accounts",
                newName: "IX_Accounts_username");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AspNetUsers_username",
                table: "Accounts",
                column: "username",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AspNetUsers_username",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Accounts",
                newName: "id_user");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_username",
                table: "Accounts",
                newName: "IX_Accounts_id_user");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AspNetUsers_id_user",
                table: "Accounts",
                column: "id_user",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
