using System;
using System.Linq;
using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data.Repositories.EntityFramework;
using FlixStats.Data.Repositories.Abstractions;
using FlixStats.Models.EntityFrameworkModels;

namespace FlixStats.Data.Repositories
{
    public class LeaderboardRepository :
        GenericEntityInterface<LeaderboardItem, StatsContext>,
        ILeaderboardRepository
    {
        public LeaderboardRepository(StatsContext context) : base(context)
        {
        }

        public IOrderedQueryable<LeaderboardItem> GetAllOrdered()
        {
            return Context.LeaderboardItems
                .OrderByDescending(x => x.TotalPlaybackTime);
        }

        public async Task CreateItemAsync(string username, string countryCode, int playbackTime)
        {
            await Context.AddAsync(new LeaderboardItem
            {
                Username = username,
                UserCountry = countryCode,
                TotalPlaybackTime = playbackTime,
                PublisheDateTime = DateTime.Now
            });

            await Context.SaveChangesAsync();
        }
    }
}
