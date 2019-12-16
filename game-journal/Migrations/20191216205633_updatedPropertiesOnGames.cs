using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace game_journal.Migrations
{
    public partial class updatedPropertiesOnGames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "_releaseDate",
                table: "Games",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "first_release_date",
                table: "Games",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "_releaseDate",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "first_release_date",
                table: "Games");
        }
    }
}
