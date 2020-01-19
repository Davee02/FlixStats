using System.Linq;
using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data.Repositories.Abstractions;
using FlixStats.Models.EntityFrameworkModels;

namespace FlixStats.Data.Repositories.Abstractions
{
    public interface ILeaderboardRepository : IGenericRepository<LeaderboardItem>
    {
        IOrderedQueryable<LeaderboardItem> GetAllOrdered();

        Task CreateItemAsync(string username, string countryCode, int playbackTime);
    }
}
