using Microsoft.EntityFrameworkCore.Migrations;

namespace game_journal.Migrations
{
    public partial class updateAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiPlatformId",
                table: "GamePlatforms");

            migrationBuilder.DropColumn(
                name: "ApiGenreId",
                table: "GameGenres");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApiPlatformId",
                table: "GamePlatforms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApiGenreId",
                table: "GameGenres",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
