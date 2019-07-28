using System.Linq;
using DaHo.Library.AspNetCore.Data.Repositories.EntityFramework;
using FlixStats.Models.EntityFrameworkModels;

namespace FlixStats.Data.Repositories.Abstractions
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
    }
}
