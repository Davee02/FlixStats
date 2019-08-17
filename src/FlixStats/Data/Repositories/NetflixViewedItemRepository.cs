using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data.Repositories.EntityFramework;
using FlixStats.Data.Repositories.Abstractions;
using FlixStats.Models.EntityFrameworkModels;
using Microsoft.EntityFrameworkCore;

namespace FlixStats.Data.Repositories
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
            return (await Context.QueryResults
                    .Include(x => x.NetflixViewedItems)
                    .FirstOrDefaultAsync(x => x.Identifier == guid))
                ?.NetflixViewedItems;
        }

        public async Task<IEnumerable<NetflixViewedItem>> GetByGuidForDayAsync(Guid guid, DateTime date)
        {
            return (await Context.QueryResults
                    .Include(x => x.NetflixViewedItems)
                    .FirstOrDefaultAsync(x => x.Identifier == guid))
                ?.NetflixViewedItems
                .Where(x => x.PlaybackDateTime.Date == date.Date);
        }

        public async Task<Guid> CreateManyWithGuidAsync(IEnumerable<NetflixViewedItem> entities)
        {
            var queryResult = new QueryResult
            {
                Identifier = Guid.NewGuid(),
                KeepResults = false,
                QueryDateTime = DateTime.Now,
                NetflixViewedItems = entities.ToList()
            };

            await Context.QueryResults.AddAsync(queryResult);
            await Context.SaveChangesAsync();

            return queryResult.Identifier;
        }

        public async Task<int> DeleteOldResultsAsync()
        {
            var toBeDeleted = await Context.QueryResults
                .Where(x => !x.KeepResults && x.QueryDateTime < DateTime.Now.AddHours(-12))
                .ToListAsync();

            Context.QueryResults.RemoveRange(toBeDeleted);

            return await Context.SaveChangesAsync();
        }

        public async Task SetKeepResultsStateAsync(Guid guid)
        {
            var toBeChanged = await Context.QueryResults
                .FirstOrDefaultAsync(x => x.Identifier == guid);

            toBeChanged.KeepResults = true;

            Context.Update(toBeChanged);
            await Context.SaveChangesAsync();
        }

        public async Task<int> GetTotalPlaybackTimeAsync(Guid guid)
        {
            return (await Context.QueryResults
                    .Include(x => x.NetflixViewedItems)
                    .FirstOrDefaultAsync(x => x.Identifier == guid))
                .NetflixViewedItems
                .Sum(x => x.PlaybackBookmark);
        }
    }
}
