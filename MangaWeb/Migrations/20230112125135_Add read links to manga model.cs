using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class Addreadlinkstomangamodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReadLinks",
                table: "Manga",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadLinks",
                table: "Manga");
        }
    }
}
