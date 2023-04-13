using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class Commentmodelupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorImgSrc",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "AuthorName",
                table: "Comments",
                newName: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Comments",
                newName: "AuthorName");

            migrationBuilder.AddColumn<string>(
                name: "AuthorImgSrc",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
