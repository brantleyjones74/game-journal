using Microsoft.EntityFrameworkCore.Migrations;

namespace game_journal.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApiPlatformId",
                table: "GamePlatforms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApiGenreId",
                table: "GameGenres",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiPlatformId",
                table: "GamePlatforms");

            migrationBuilder.DropColumn(
                name: "ApiGenreId",
                table: "GameGenres");
        }
    }
}
