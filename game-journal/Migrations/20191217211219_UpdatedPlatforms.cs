using Microsoft.EntityFrameworkCore.Migrations;

namespace game_journal.Migrations
{
    public partial class UpdatedPlatforms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApiPlatformId",
                table: "Platforms",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiPlatformId",
                table: "Platforms");
        }
    }
}
