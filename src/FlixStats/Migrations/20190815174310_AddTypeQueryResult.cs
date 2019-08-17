using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlixStats.Migrations
{
    public partial class AddTypeQueryResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "NetflixViewedItems");

            migrationBuilder.DropColumn(
                name: "KeepResult",
                table: "NetflixViewedItems");

            migrationBuilder.DropColumn(
                name: "SavedDateTime",
                table: "NetflixViewedItems");

            migrationBuilder.AddColumn<long>(
                name: "QueryResultId",
                table: "NetflixViewedItems",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QueryResults",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Identifier = table.Column<Guid>(nullable: false),
                    QueryDateTime = table.Column<DateTime>(nullable: false),
                    KeepResults = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryResults", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NetflixViewedItems_QueryResults_QueryResultId",
                table: "NetflixViewedItems");

            migrationBuilder.DropTable(
                name: "QueryResults");

            migrationBuilder.DropIndex(
                name: "IX_NetflixViewedItems_QueryResultId",
                table: "NetflixViewedItems");

            migrationBuilder.DropColumn(
                name: "QueryResultId",
                table: "NetflixViewedItems");

            migrationBuilder.AddColumn<Guid>(
                name: "Identifier",
                table: "NetflixViewedItems",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
    }
}
