using System.Linq;
using DaHo.Library.AspNetCore.Data.Repositories.Abstractions;
using FlixStats.Models.EntityFrameworkModels;

namespace FlixStats.Data.Repositories.Abstractions
{
    public interface ILeaderboardRepository : IGenericInterface<LeaderboardItem>
    {
        IOrderedQueryable<LeaderboardItem> GetAllOrdered();
    }
}
