using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class addunsignedtitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnsignedTitle",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnsignedTitle",
                table: "Posts");
        }
    }
}
