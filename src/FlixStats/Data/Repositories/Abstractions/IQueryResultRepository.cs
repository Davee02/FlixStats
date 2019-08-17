using System;
using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data.Repositories.Abstractions;
using FlixStats.Models.EntityFrameworkModels;

namespace FlixStats.Data.Repositories.Abstractions
{
    public interface IQueryResultRepository : IGenericInterface<QueryResult>
    {
        Task<QueryResult> GetByGuidWithoutViewedItemsAsync(Guid guid);
    }
}
