﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data.Repositories.EntityFramework;
using Microsoft.EntityFrameworkCore;
using NetflixStatizier.Data.Repositories.Abstractions;
using NetflixStatizier.Models.EntityFrameworkModels;

namespace NetflixStatizier.Data.Repositories
{
    public class NetflixViewedItemRepository : GenericEntityInterface<NetflixViewedItem, StatsContext>, INetflixViewedItemRepository
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

        public async Task CreateManyAsync(IEnumerable<NetflixViewedItem> entities)
        {
            await Context.AddRangeAsync(entities);
            await Context.SaveChangesAsync();
        }
    }
}
