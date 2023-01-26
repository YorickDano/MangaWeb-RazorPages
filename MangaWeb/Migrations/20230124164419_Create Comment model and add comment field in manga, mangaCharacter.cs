using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class CreateCommentmodelandaddcommentfieldinmangamangaCharacter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    MangaId = table.Column<int>(type: "int", nullable: true),
                    CharacterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Manga_MangaId",
                        column: x => x.MangaId,
                        principalTable: "Manga",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_MangaCharacter_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "MangaCharacter",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CharacterId",
                table: "Comments",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_MangaId",
                table: "Comments",
                column: "MangaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
