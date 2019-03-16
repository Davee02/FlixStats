using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Models;

namespace NetflixStatizier.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly UserManager<IdentityUser> UserManager;

        protected BaseController(UserManager<IdentityUser> userManager)
        {
            UserManager = userManager;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
