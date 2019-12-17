using Microsoft.EntityFrameworkCore.Migrations;

namespace game_journal.Migrations
{
    public partial class updatedGenresTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApiGenreId",
                table: "Genres",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiGenreId",
                table: "Genres");
        }
    }
}
