using FlixStats.Models.EntityFrameworkModels;
using Microsoft.EntityFrameworkCore;

namespace FlixStats.Data
{
    public class StatsContext : DbContext
    {
        public StatsContext(DbContextOptions<StatsContext> options) : base(options) { }

        public StatsContext() { }

        public DbSet<NetflixViewedItem> NetflixViewedItems { get; set; }

        public DbSet<LeaderboardItem> LeaderboardItems { get; set; }

        public DbSet<QueryResult> QueryResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QueryResult>()
                .HasMany(x => x.NetflixViewedItems)
                .WithOne(x => x.Query)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
