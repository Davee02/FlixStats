using FlixStats.Models.EntityFrameworkModels;
using Microsoft.EntityFrameworkCore;

namespace FlixStats.Data
{
    public class StatsContext : DbContext
    {
        public StatsContext(DbContextOptions options) : base(options) { }

        public DbSet<NetflixViewedItem> NetflixViewedItems { get; set; }
    }
}
