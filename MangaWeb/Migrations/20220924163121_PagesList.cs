using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class PagesList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MangaCharacter_FullMangas_FullMangaId",
                table: "MangaCharacter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FullMangas",
                table: "FullMangas");

            migrationBuilder.RenameTable(
                name: "FullMangas",
                newName: "FullManga");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FullManga",
                table: "FullManga",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MangaRead",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VolumeNumber = table.Column<int>(type: "int", nullable: false),
                    ChapterNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaRead", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MangaReadPage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageNumber = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MangaReadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaReadPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MangaReadPage_MangaRead_MangaReadId",
                        column: x => x.MangaReadId,
                        principalTable: "MangaRead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MangaReadPage_MangaReadId",
                table: "MangaReadPage",
                column: "MangaReadId");

            migrationBuilder.AddForeignKey(
                name: "FK_MangaCharacter_FullManga_FullMangaId",
                table: "MangaCharacter",
                column: "FullMangaId",
                principalTable: "FullManga",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MangaCharacter_FullManga_FullMangaId",
                table: "MangaCharacter");

            migrationBuilder.DropTable(
                name: "MangaReadPage");

            migrationBuilder.DropTable(
                name: "MangaRead");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FullManga",
                table: "FullManga");

            migrationBuilder.RenameTable(
                name: "FullManga",
                newName: "FullMangas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FullMangas",
                table: "FullMangas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MangaCharacter_FullMangas_FullMangaId",
                table: "MangaCharacter",
                column: "FullMangaId",
                principalTable: "FullMangas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
