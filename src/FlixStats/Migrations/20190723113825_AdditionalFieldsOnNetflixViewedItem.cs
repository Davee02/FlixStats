using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlixStats.Migrations
{
    public partial class AdditionalFieldsOnNetflixViewedItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "KeepResult",
                table: "NetflixViewedItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SavedDateTime",
                table: "NetflixViewedItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeepResult",
                table: "NetflixViewedItems");

            migrationBuilder.DropColumn(
                name: "SavedDateTime",
                table: "NetflixViewedItems");
        }
    }
}
