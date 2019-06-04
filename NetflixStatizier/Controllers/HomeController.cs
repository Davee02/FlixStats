using Microsoft.AspNetCore.Mvc;

namespace NetflixStatizier.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
