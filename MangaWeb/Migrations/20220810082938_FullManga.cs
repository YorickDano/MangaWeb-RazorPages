using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class FullManga : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FullMangas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MangaImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Published = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountOfVolume = table.Column<int>(type: "int", nullable: true),
                    CountOfChapters = table.Column<int>(type: "int", nullable: true),
                    Score = table.Column<double>(type: "float", nullable: false),
                    Ranked = table.Column<int>(type: "int", nullable: false),
                    Popularity = table.Column<int>(type: "int", nullable: false),
                    Geners = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Autors = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FullMangas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MangaCharacter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    FullMangaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaCharacter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MangaCharacter_FullMangas_FullMangaId",
                        column: x => x.FullMangaId,
                        principalTable: "FullMangas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MangaCharacter_FullMangaId",
                table: "MangaCharacter",
                column: "FullMangaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MangaCharacter");

            migrationBuilder.DropTable(
                name: "FullMangas");
        }
    }
}
