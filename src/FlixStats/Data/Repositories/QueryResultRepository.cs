using System;
using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data.Repositories.EntityFramework;
using FlixStats.Data.Repositories.Abstractions;
using FlixStats.Models.EntityFrameworkModels;
using Microsoft.EntityFrameworkCore;

namespace FlixStats.Data.Repositories
{
    public class QueryResultRepository : 
        GenericEntityRepository<QueryResult, StatsContext>,
        IQueryResultRepository
    {
        public QueryResultRepository(StatsContext context) : base(context)
        {
        }

        public async Task<QueryResult> GetByGuidWithoutViewedItemsAsync(Guid guid)
        {
            return await Context.QueryResults.FirstOrDefaultAsync(x => x.Identifier == guid);
        }
    }
}
