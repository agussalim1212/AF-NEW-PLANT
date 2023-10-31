using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkeletonApi.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class part20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryMachineHasMachine_CategoryMachine_CategoryMachineId",
                table: "CategoryMachineHasMachine");

            migrationBuilder.DropTable(
                name: "CategoryMachine");

            migrationBuilder.CreateTable(
                name: "CategoryMachines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    update_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryMachines", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryMachineHasMachine_CategoryMachines_CategoryMachineId",
                table: "CategoryMachineHasMachine",
                column: "CategoryMachineId",
                principalTable: "CategoryMachines",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryMachineHasMachine_CategoryMachines_CategoryMachineId",
                table: "CategoryMachineHasMachine");

            migrationBuilder.DropTable(
                name: "CategoryMachines");

            migrationBuilder.CreateTable(
                name: "CategoryMachine",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    update_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryMachine", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryMachineHasMachine_CategoryMachine_CategoryMachineId",
                table: "CategoryMachineHasMachine",
                column: "CategoryMachineId",
                principalTable: "CategoryMachine",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
