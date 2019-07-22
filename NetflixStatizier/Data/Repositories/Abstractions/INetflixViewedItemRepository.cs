using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data.Repositories.Abstractions;
using NetflixStatizier.Models.EntityFrameworkModels;

namespace NetflixStatizier.Data.Repositories.Abstractions
{
    public interface INetflixViewedItemRepository : IGenericInterface<NetflixViewedItem>
    {
        Task<IEnumerable<NetflixViewedItem>> GetByGuidAsync(Guid guid);

        Task<IEnumerable<NetflixViewedItem>> GetByGuidForDayAsync(Guid guid, DateTime date);

        Task<Guid> CreateManyWithGuidAsync(IEnumerable<NetflixViewedItem> entities);
    }
}
