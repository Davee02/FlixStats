using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlixStats.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NetflixViewedItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Identifier = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    VideoTitle = table.Column<string>(nullable: true),
                    MovieId = table.Column<int>(nullable: false),
                    CountryCode = table.Column<string>(nullable: true),
                    PlaybackBookmark = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    PlaybackDateTime = table.Column<DateTime>(nullable: false),
                    DeviceType = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    TopNodeId = table.Column<string>(nullable: true),
                    SeriesId = table.Column<int>(nullable: false),
                    SeriesTitle = table.Column<string>(nullable: true),
                    SeasonDescriptor = table.Column<string>(nullable: true),
                    EpisodeTitle = table.Column<string>(nullable: true),
                    EstRating = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetflixViewedItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NetflixViewedItems");
        }
    }
}
