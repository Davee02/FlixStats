using Microsoft.EntityFrameworkCore;
using NetflixStatizier.Models.EntityFrameworkModels;

namespace NetflixStatizier.Data
{
    public class StatsContext : DbContext
    {
        public StatsContext(DbContextOptions options) : base(options) { }

        public DbSet<NetflixViewedItem> NetflixViewedItems { get; set; }
    }
}
