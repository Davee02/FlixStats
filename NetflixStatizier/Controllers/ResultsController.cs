using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Models;
using ChartJSCore.Models;
using ChartJSCore.Models.Bar;
using Microsoft.AspNetCore.Identity;
using NetflixStatizier.Interfaces;
using NetflixStatizier.Stats;
using NetflixStatizier.Stats.Exceptions;
using NetflixStatizier.Stats.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Enums = ChartJSCore.Models.Enums;
using Time = NetflixStatizier.Helper.Time;

namespace NetflixStatizier.Controllers
{
    public class ResultsController : BaseController
    {
        private readonly INetflixAccountRepository m_NetflixAccountRepository;


        public ResultsController(INetflixAccountRepository netflixAccountRepository, UserManager<IdentityUser> userManager)
            : base(userManager)
        {
            m_NetflixAccountRepository = netflixAccountRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetStats(NetflixAccountModel model)
        {
            if (!ModelState.IsValid)
                return View("../Home/Index", model);

            using (var driver = GetWebDriver())
            {
                //var stats = new NetflixViewingHistoryLoader("kiumo777@gmail.com", "s-INF17a+");
                var stats = new NetflixViewingHistoryLoader(model.NetflixEmail, model.NetflixPassword);
                var history = await stats.LoadNetflixViewingHistoryAsync(model.NetflixProfileName, driver);

                var calculatedStats = CalculateNetflixStats(history);

                return View("Index", calculatedStats);
            }

        }


        private static IWebDriver GetWebDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("blink-settings=imagesEnabled=false");
            options.AddArgument("disable-gpu");
            return new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
        }

        private static NetflixStatsModel CalculateNetflixStats(IEnumerable<NetflixPlayback> viewingHistory)
        {
            var statsCalculator = new NetflixStatsCalculator(viewingHistory);

            var viewedHoursPerSerie = statsCalculator.GetViewedMinutesPerSerie()
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => $"{x.Key} - {Math.Round(x.Value / 60, 2)}h", x => (double)Math.Round(x.Value / 60, 2));

            var viewedHoursPerDay = statsCalculator.GetViewedMinutesPerDay()
                .OrderBy(x => x.Key)
                .ToDictionary(x => $"{x.Key:d} - {Math.Round(x.Value / 60, 2)}h", x => (double)Math.Round(x.Value / 60, 2));


            var statsModel = new NetflixStatsModel
            {
                TotalViewedTime = Time.FromMinutes((double)statsCalculator.GetTotalViewedMinutes()),
                MoviesViewedTime = Time.FromMinutes((double)statsCalculator.GetMoviesViewedMinutes()),
                SeriesViewedTime = Time.FromMinutes((double)statsCalculator.GetSeriesEpisodesViewedMinutes()),
                MoviesViewedCount = statsCalculator.GetMoviesViewedCount(),
                SeriesEpisodesViewedItemsCount = statsCalculator.GetSeriesEpisodesViewedCount(),
                FirstWatchedMovie = statsCalculator.GetFirstWatchedMovie(),
                FirstWatchedSeriesEpisode = statsCalculator.GetFirstWatchedSeriesEpisode(),
                ViewedHoursPerSerieChart = GetTimePerSerieChart(viewedHoursPerSerie),
                ViewedHoursPerDayChart = GetTimePerDayChart(viewedHoursPerDay),
            };


            return statsModel;
        }

        private static Chart GetTimePerSerieChart(Dictionary<string, double> timePerSerie)
        {
            var chart = new Chart { Type = Enums.ChartType.HorizontalBar };
            var data = new ChartJSCore.Models.Data { Labels = new List<string>(timePerSerie.Keys) };
            var dataset = new BarDataset
            {
                Label = "# hours watched",
                Data = new List<double>(timePerSerie.Values),
                BorderWidth = new List<int> { 1 },
                BackgroundColor = new List<string> { "rgb(159, 154, 232)" },
                Type = Enums.ChartType.HorizontalBar
            };
            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;
            chart.Options = new BarOptions
            {
                Responsive = true,
                Title = new Title { Text = "Hours watched per serie" }
            };

            return chart;
        }

        private static Chart GetTimePerDayChart(Dictionary<string, double> timePerSerie)
        {
            var chart = new Chart { Type = Enums.ChartType.HorizontalBar };
            var data = new ChartJSCore.Models.Data { Labels = new List<string>(timePerSerie.Keys) };
            var dataset = new BarDataset
            {
                Label = "# hours watched",
                Data = new List<double>(timePerSerie.Values),
                BorderWidth = new List<int> { 1 }
            };
            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;

            return chart;
        }
    }
}