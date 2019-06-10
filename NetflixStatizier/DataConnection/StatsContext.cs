using Microsoft.EntityFrameworkCore;
using NetflixStatizier.Stats.Model;

namespace NetflixStatizier.DataConnection
{
    public class StatsContext : DbContext
    {
        public StatsContext(DbContextOptions options) : base(options) { }


        public DbSet<NetflixPlayback> NetflixPlaybacks { get; set; }
    }
}
