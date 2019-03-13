using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Models;

namespace NetflixStatizier.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveNetflixAccount(NetflixAccountModel netflixAccount)
        {
            
        }
    }
}
