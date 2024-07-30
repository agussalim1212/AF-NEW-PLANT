using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkeletonApi.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class part12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "update_at",
                table: "FrameNumberHasSubject",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "update_at",
                table: "CategoryMachineHasMachine",
                newName: "updated_at");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "SubjectHasMachine",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "FrameNumberHasSubject",
                newName: "update_at");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "CategoryMachineHasMachine",
                newName: "update_at");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "SubjectHasMachine",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}