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
        private const decimal SECONDS_PER_HOUR = 60 * 60;

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

        private static NetflixStatsModel CalculateNetflixStats(IEnumerable<NetflixViewingHistoryPart> viewingHistory)
        {
            var allViewedElements = viewingHistory
                .SelectMany(x => x.ViewedItems)
                .ToList();

            var allMovies = allViewedElements
                .Where(x => x.SeriesId == 0)
                .ToList();
            var allSeriesEpisodes = allViewedElements
                .Where(x => x.SeriesId != 0)
                .ToList();

            var statsModel = new NetflixStatsModel
            {
                TotalViewedHours = allViewedElements.Sum(x => x.PlaybackBookmark) / SECONDS_PER_HOUR,
                MoviesViewedHours = allMovies.Sum(x => x.PlaybackBookmark) / SECONDS_PER_HOUR,
                SeriesViewedHours = allSeriesEpisodes.Sum(x => x.PlaybackBookmark) / SECONDS_PER_HOUR,
                FirstWatchedItemDateTime = allViewedElements.OrderBy(x => x.PlaybackDateTime).First().PlaybackDateTime,
                LastWatchedItemDateTime = allViewedElements.OrderByDescending(x => x.PlaybackDateTime).First().PlaybackDateTime,
                MoviesViewedCount = allMovies.Count,
                SeriesEpisodesViewedItemsCount = allSeriesEpisodes.Count
            };

            return statsModel;
        }
    }
}
