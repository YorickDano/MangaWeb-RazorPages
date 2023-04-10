using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class Conversationuserinfochange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstUserImageSrc",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "FirstUserName",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SecondUserImageSrc",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SecondUserName",
                table: "Conversations");

            migrationBuilder.AddColumn<string>(
                name: "FirstUserId",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondUserId",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_FirstUserId",
                table: "Conversations",
                column: "FirstUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_SecondUserId",
                table: "Conversations",
                column: "SecondUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_FirstUserId",
                table: "Conversations",
                column: "FirstUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_SecondUserId",
                table: "Conversations",
                column: "SecondUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_FirstUserId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_SecondUserId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_FirstUserId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_SecondUserId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "FirstUserId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SecondUserId",
                table: "Conversations");

            migrationBuilder.AddColumn<string>(
                name: "FirstUserImageSrc",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstUserName",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondUserImageSrc",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondUserName",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
