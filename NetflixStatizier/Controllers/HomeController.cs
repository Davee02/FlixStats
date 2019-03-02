using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Models;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NetflixStatizier.Stats;
using NetflixStatizier.Stats.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace NetflixStatizier.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetStats(NetflixAccountModel model)
        {
            using (var driver = GetWebDriver())
            {
                //var stats = new NetflixStats("kiumo777@gmail.com", "s-INF17a+");
                var stats = new NetflixStats(model.NetflixEmail, model.NetflixPassword);
                var history = await stats.GetNetflixViewingHistory("dave", driver);

                var calculatedStats = CalculateNetflixStats(history);
                return Ok(history);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static IWebDriver GetWebDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("blink-settings=imagesEnabled=false");
            options.AddArgument("disable-gpu");
            return new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
        }

        private static NetflixStatsModel CalculateNetflixStats(ICollection<NetflixViewingHistoryPart> viewingHistory)
        {
            var allViewedElements = viewingHistory
                .SelectMany(x => x.ViewedItems)
                .ToList();

            var statsModel = new NetflixStatsModel();
            statsModel.TotalViewedHours = (decimal)(allViewedElements.Sum(x => x.Bookmark) / 60.0 / 60.0);
            statsModel.FirstWatchedItemDateTime = allViewedElements.OrderBy(x => x.Date).First().Date;
            statsModel.LastWatchedItemDateTime = allViewedElements.OrderByDescending(x => x.Date).First().Date;
            statsModel.MoviesViewedItemsCount = allViewedElements.Count(x => x.Series == 0);

            return statsModel;
        }
    }
}
