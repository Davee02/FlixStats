using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Data;
using NetflixStatizier.Models;

namespace NetflixStatizier.Controllers
{
    public class HomeController : BaseController
    {
        private readonly StatsContext _statsContext;

        public HomeController(UserManager<IdentityUser> userManager, StatsContext statsContext)
            : base(userManager)
        {
            _statsContext = statsContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveNetflixAccount(NetflixAccountModel netflixAccount)
        {
            await _statsContext.NetflixAccounts.AddAsync(netflixAccount);
            await _statsContext.SaveChangesAsync();

            return View();
        }
    }
}
