using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkeletonApi.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class part98 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "ActivityUsers",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "LogType",
                table: "ActivityUsers",
                newName: "logtype");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "ActivityUsers",
                newName: "datetime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ActivityUsers",
                newName: "id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "datetime",
                table: "ActivityUsers",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "username",
                table: "ActivityUsers",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "logtype",
                table: "ActivityUsers",
                newName: "LogType");

            migrationBuilder.RenameColumn(
                name: "datetime",
                table: "ActivityUsers",
                newName: "DateTime");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ActivityUsers",
                newName: "Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "ActivityUsers",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp");
        }
    }
}
