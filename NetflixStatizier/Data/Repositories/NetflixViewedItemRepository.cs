using DaHo.Library.AspNetCore.Data.Repositories.EntityFramework;
using Microsoft.EntityFrameworkCore;
using NetflixStatizier.Data.Repositories.Abstractions;
using NetflixStatizier.Models.EntityFrameworkModels;

namespace NetflixStatizier.Data.Repositories
{
    public class NetflixViewedItemRepository : GenericEntityInterface<NetflixViewedItem>, INetflixViewedItemRepository
    {
        public NetflixViewedItemRepository(DbContext context) : base(context)
        {
        }
    }
}
