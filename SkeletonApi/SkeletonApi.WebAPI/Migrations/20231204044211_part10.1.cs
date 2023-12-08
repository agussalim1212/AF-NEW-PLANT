using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkeletonApi.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class part101 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AspNetUsers_username",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_username",
                table: "Accounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Accounts_username",
                table: "Accounts",
                column: "username");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AspNetUsers_username",
                table: "Accounts",
                column: "username",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
