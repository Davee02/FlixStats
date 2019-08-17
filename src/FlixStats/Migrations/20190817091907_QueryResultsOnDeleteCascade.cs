using Microsoft.EntityFrameworkCore.Migrations;

namespace FlixStats.Migrations
{
    public partial class QueryResultsOnDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NetflixViewedItems_QueryResults_QueryResultId",
                table: "NetflixViewedItems");

            migrationBuilder.DropIndex(
                name: "IX_NetflixViewedItems_QueryResultId",
                table: "NetflixViewedItems");

            migrationBuilder.DropColumn(
                name: "QueryResultId",
                table: "NetflixViewedItems");

            migrationBuilder.AddColumn<long>(
                name: "QueryId",
                table: "NetflixViewedItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetflixViewedItems_QueryId",
                table: "NetflixViewedItems",
                column: "QueryId");

            migrationBuilder.AddForeignKey(
                name: "FK_NetflixViewedItems_QueryResults_QueryId",
                table: "NetflixViewedItems",
                column: "QueryId",
                principalTable: "QueryResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NetflixViewedItems_QueryResults_QueryId",
                table: "NetflixViewedItems");

            migrationBuilder.DropIndex(
                name: "IX_NetflixViewedItems_QueryId",
                table: "NetflixViewedItems");

            migrationBuilder.DropColumn(
                name: "QueryId",
                table: "NetflixViewedItems");

            migrationBuilder.AddColumn<long>(
                name: "QueryResultId",
                table: "NetflixViewedItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetflixViewedItems_QueryResultId",
                table: "NetflixViewedItems",
                column: "QueryResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_NetflixViewedItems_QueryResults_QueryResultId",
                table: "NetflixViewedItems",
                column: "QueryResultId",
                principalTable: "QueryResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
