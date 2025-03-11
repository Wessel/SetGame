using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class HintsnFound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "Found",
                table: "Games",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Hints",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Found",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Hints",
                table: "Games");
        }
    }
}
