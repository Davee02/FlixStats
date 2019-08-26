using Microsoft.EntityFrameworkCore.Migrations;

namespace FlixStats.Migrations
{
    public partial class SaveLeaderboardStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublishedToLeaderboard",
                table: "QueryResults",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublishedToLeaderboard",
                table: "QueryResults");
        }
    }
}
