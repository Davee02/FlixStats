using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Data;
using NetflixStatizier.Models;

namespace NetflixStatizier.Controllers
{
    public class HomeController : BaseController
    {
        private readonly StatsContext m_StatsContext;

        public HomeController(StatsContext statsContext)
        {
            m_StatsContext = statsContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveNetflixAccount(NetflixAccountModel netflixAccount)
        {
            await m_StatsContext.NetflixAccounts.AddAsync(netflixAccount);
            await m_StatsContext.SaveChangesAsync();

            return View();
        }
    }
}
