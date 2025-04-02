using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetTutorial.Migrations
{
    /// <inheritdoc />
    public partial class addList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "Tags",
                table: "Users",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Users");
        }
    }
}
