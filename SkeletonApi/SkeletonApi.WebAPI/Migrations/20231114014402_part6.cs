using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkeletonApi.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class part6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "vid",
                table: "Machines",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "vid",
                table: "Machines");
        }
    }
}
