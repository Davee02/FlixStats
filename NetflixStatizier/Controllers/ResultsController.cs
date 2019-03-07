using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Models;

namespace NetflixStatizier.Controllers
{
    public class ResultsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DisplayNetflixStats(NetflixStatsModel model)
        {
            return Ok(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}