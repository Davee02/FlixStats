using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data.Repositories.EntityFramework;
using Microsoft.EntityFrameworkCore;
using NetflixStatizier.Data.Repositories.Abstractions;
using NetflixStatizier.Models.EntityFrameworkModels;

namespace NetflixStatizier.Data.Repositories
{
    public class NetflixViewedItemRepository :
        GenericEntityInterface<NetflixViewedItem, StatsContext>,
        INetflixViewedItemRepository
    {
        public NetflixViewedItemRepository(StatsContext context) : base(context)
        {
        }

        public async Task<IEnumerable<NetflixViewedItem>> GetByGuidAsync(Guid guid)
        {
            return await Context.NetflixViewedItems
                .Where(x => x.Identifier == guid)
                .ToListAsync();
        }

        public async Task<IEnumerable<NetflixViewedItem>> GetByGuidForDayAsync(Guid guid, DateTime date)
        {
            return await Context.NetflixViewedItems
                .Where(x => x.Identifier == guid && x.PlaybackDateTime.Date == date.Date)
                .ToListAsync();
        }

        public async Task<Guid> CreateManyWithGuidAsync(IEnumerable<NetflixViewedItem> entities)
        {
            var identificationGuid = Guid.NewGuid();
            var createdDateTime = DateTime.Now;

            foreach (var entity in entities)
            {
                entity.Identifier = identificationGuid;
                entity.SavedDateTime = createdDateTime;
                entity.KeepResult = false;
            }

            await Context.AddRangeAsync(entities);
            await Context.SaveChangesAsync();

            return identificationGuid;
        }

        public async Task<int> DeleteOldResultsAsync()
        {
            var toBeDeleted = await Context.NetflixViewedItems
                .Where(x => !x.KeepResult && x.SavedDateTime < DateTime.Now.AddHours(-12))
                .ToListAsync();

            Context.NetflixViewedItems.RemoveRange(toBeDeleted);

            return await Context.SaveChangesAsync();
        }

        public async Task SetKeepResultsStateAsync(Guid guid)
        {
            var toBeChanged = await GetByGuidAsync(guid);

            foreach (var entity in toBeChanged)
            {
                entity.KeepResult = true;
            }

            Context.UpdateRange(toBeChanged);
            await Context.SaveChangesAsync();
        }
    }
}
