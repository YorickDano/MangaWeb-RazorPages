using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class mangaCharacterFullMangaAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MangaCharacter_FullMangas_FullMangaId",
                table: "MangaCharacter");

            migrationBuilder.AlterColumn<int>(
                name: "FullMangaId",
                table: "MangaCharacter",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MangaCharacter_FullMangas_FullMangaId",
                table: "MangaCharacter",
                column: "FullMangaId",
                principalTable: "FullMangas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MangaCharacter_FullMangas_FullMangaId",
                table: "MangaCharacter");

            migrationBuilder.AlterColumn<int>(
                name: "FullMangaId",
                table: "MangaCharacter",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MangaCharacter_FullMangas_FullMangaId",
                table: "MangaCharacter",
                column: "FullMangaId",
                principalTable: "FullMangas",
                principalColumn: "Id");
        }
    }
}
