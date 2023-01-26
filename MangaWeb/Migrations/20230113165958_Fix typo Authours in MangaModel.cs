using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWeb.Migrations
{
    public partial class FixtypoAuthoursinMangaModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("Authors", "Manga", "Authors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("Authors", "Manga", "Authors");
        }
    }
}
