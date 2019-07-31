using Microsoft.AspNetCore.Mvc;

namespace FlixStats.Controllers
{
    public class FaqController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
