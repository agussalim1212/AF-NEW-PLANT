using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkeletonApi.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class part15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListQualityAutoQGates");

            migrationBuilder.CreateTable(
                name: "ListQualityBarcodes",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    data_barcode = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    FotoDataNg = table.Column<string>(type: "text", nullable: true),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListQualityBarcodes");

            migrationBuilder.CreateTable(
                name: "ListQualityAutoQGates",
                columns: table => new
                {
                    data_barcode = table.Column<string>(type: "text", nullable: false),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    id = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                });
        }
    }
}
