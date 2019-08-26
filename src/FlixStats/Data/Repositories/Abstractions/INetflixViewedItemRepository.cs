using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data.Repositories.Abstractions;
using FlixStats.Models.EntityFrameworkModels;

namespace FlixStats.Data.Repositories.Abstractions
{
    public interface INetflixViewedItemRepository : IGenericInterface<NetflixViewedItem>
    {
        Task<IEnumerable<NetflixViewedItem>> GetByGuidAsync(Guid guid);

        Task<IEnumerable<NetflixViewedItem>> GetByGuidForDayAsync(Guid guid, DateTime date);

        Task<Guid> CreateManyWithGuidAsync(IEnumerable<NetflixViewedItem> entities);

        Task<int> DeleteOldResultsAsync();

        Task SetKeepResultsStateAsync(Guid guid);

        Task SetPublishedToLeaderboardStateAsync(Guid guid);

        Task<int> GetTotalPlaybackTimeAsync(Guid guid);
    }
}
