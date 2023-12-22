using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkeletonApi.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class part99 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "NoNRP",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "PhotoURL",
                table: "Accounts",
                newName: "photo_url");

            migrationBuilder.AddColumn<string>(
                name: "id_user",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_id_user",
                table: "Accounts",
                column: "id_user");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AspNetUsers_id_user",
                table: "Accounts",
                column: "id_user",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AspNetUsers_id_user",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_id_user",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "id_user",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "photo_url",
                table: "Accounts",
                newName: "PhotoURL");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Accounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClubId",
                table: "Accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NoNRP",
                table: "Accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
