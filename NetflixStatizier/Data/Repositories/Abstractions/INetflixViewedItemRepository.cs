using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data.Repositories.Abstractions;
using NetflixStatizier.Models.EntityFrameworkModels;

namespace NetflixStatizier.Data.Repositories.Abstractions
{
    public interface INetflixViewedItemRepository : IGenericInterface<NetflixViewedItem>
    {
        Task<IEnumerable<NetflixViewedItem>> GetByGuid(Guid guid);
    }
}
