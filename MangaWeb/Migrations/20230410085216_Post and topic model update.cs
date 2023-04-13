using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class Postandtopicmodelupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorImgSrc",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "AuthorImgSrc",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "AuthorName",
                table: "Topics",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "AuthorName",
                table: "Posts",
                newName: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Topics",
                newName: "AuthorName");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Posts",
                newName: "AuthorName");

            migrationBuilder.AddColumn<string>(
                name: "AuthorImgSrc",
                table: "Topics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorImgSrc",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
