using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkeletonApi.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class part5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FrameNumberHasSubject_FrameNumber_frame_number_id",
                table: "FrameNumberHasSubject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FrameNumber",
                table: "FrameNumber");

            migrationBuilder.RenameTable(
                name: "FrameNumber",
                newName: "FrameNumbers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FrameNumbers",
                table: "FrameNumbers",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_FrameNumberHasSubject_FrameNumbers_frame_number_id",
                table: "FrameNumberHasSubject",
                column: "frame_number_id",
                principalTable: "FrameNumbers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FrameNumberHasSubject_FrameNumbers_frame_number_id",
                table: "FrameNumberHasSubject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FrameNumbers",
                table: "FrameNumbers");

            migrationBuilder.RenameTable(
                name: "FrameNumbers",
                newName: "FrameNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FrameNumber",
                table: "FrameNumber",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_FrameNumberHasSubject_FrameNumber_frame_number_id",
                table: "FrameNumberHasSubject",
                column: "frame_number_id",
                principalTable: "FrameNumber",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
