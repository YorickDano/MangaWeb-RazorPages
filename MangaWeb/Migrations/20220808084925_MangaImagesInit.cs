using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class MangaImagesInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Manga",
                newName: "MainImageUrl");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MangaImages");

            migrationBuilder.RenameColumn(
                name: "MainImageUrl",
                table: "Manga",
                newName: "ImageUrl");
        }
    }
}
