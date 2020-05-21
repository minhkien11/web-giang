using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class addcover : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImage",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "Posts");
        }
    }
}
