using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class MangaImagesedit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MangaImages");

            migrationBuilder.AddColumn<string>(
                name: "AnimeImagesUrls",
                table: "Manga",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HentaiImagesUrls",
                table: "Manga",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimeImagesUrls",
                table: "Manga");

            migrationBuilder.DropColumn(
                name: "HentaiImagesUrls",
                table: "Manga");

            migrationBuilder.CreateTable(
                name: "MangaImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnimeImagesUrls = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HentaiImagesUrls = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MangaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MangaImages_Manga_MangaId",
                        column: x => x.MangaId,
                        principalTable: "Manga",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MangaImages_MangaId",
                table: "MangaImages",
                column: "MangaId");
        }
    }
}
