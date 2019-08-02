using System.Threading.Tasks;
using DaHo.Library.AspNetCore.Data;
using FlixStats.Data.Repositories.Abstractions;
using FlixStats.Models.EntityFrameworkModels;
using FlixStats.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FlixStats.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardRepository _leaderboardRepository;

        public LeaderboardController(ILeaderboardRepository leaderboardRepository)
        {
            _leaderboardRepository = leaderboardRepository;
        }

        public async Task<IActionResult> Index(int pageIndex = 0)
        {
            const int pageSize = 10;

            var viewModel = new LeaderboardViewModel
            {
                LeaderboardItems = await PaginatedList<LeaderboardItem>.CreateAsync(_leaderboardRepository.GetAllOrdered(), pageIndex, pageSize),
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return View("Index", viewModel);
        }
    }
}