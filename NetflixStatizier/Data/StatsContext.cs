using Microsoft.EntityFrameworkCore;
using NetflixStatizier.Models;

namespace NetflixStatizier.Data
{
    public class StatsContext : DbContext
    {
        public StatsContext(DbContextOptions<StatsContext> options)
            : base(options)
        { }

        public DbSet<NetflixAccountModel> NetflixAccounts { get; set; }
    }
}
