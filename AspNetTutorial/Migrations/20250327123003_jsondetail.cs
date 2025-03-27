using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetTutorial.Migrations
{
    /// <inheritdoc />
    public partial class jsondetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JsonDetail",
                table: "Users",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonDetail",
                table: "Users");
        }
    }
}
