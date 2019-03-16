using NetflixStatizier.Interfaces;
using NetflixStatizier.Models;

namespace NetflixStatizier.Data.Repositories
{
    public class NetflixAccountRepository: EntityRepositoryBase<NetflixAccountModel>, INetflixAccountRepository
    {
        public NetflixAccountRepository(StatsContext context) 
            : base(context)
        {
        }
    }
}