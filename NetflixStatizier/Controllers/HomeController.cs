using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NetflixStatizier;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Models;
using NetflixStatizier.Stats;

namespace NetflixStatizier.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var stats = new NetflixStats();
            stats.GetAllNetflixPlays();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
