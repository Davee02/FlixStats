using ChartJSCore.Models;
using ChartJSCore.Models.Bar;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Models;
using NetflixStatizier.Stats;
using NetflixStatizier.Stats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixStatizier.Interfaces;
using RestEase;
using Enums = ChartJSCore.Models.Enums;
using Time = NetflixStatizier.Helper.Time;

namespace NetflixStatizier.Controllers
{
    public class ResultsController : BaseController
    {
        private readonly UserManager<IdentityUser> m_UserManager;

        public ResultsController(UserManager<IdentityUser> userManager)
            : base(userManager)
        {
            m_UserManager = userManager;
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

                var netflixApi = RestClient.For<INetflixApi>("https://localhost:5005/api");
                var history = await netflixApi.GetNetflixViewingHistory(new NetflixProfile
                {
                    AccountEmail = model.NetflixEmail, AccountPassword = model.NetflixPassword,
                    ProfileName = model.NetflixProfileName
                });

                var calculatedStats = CalculateNetflixStats(history);

                return View("Index", calculatedStats);

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

        private static Chart GetTimePerDayChart(Dictionary<string, double> timePerDay)
        {
            var chart = new Chart { Type = Enums.ChartType.HorizontalBar };
            var data = new ChartJSCore.Models.Data { Labels = new List<string>(timePerDay.Keys) };
            var dataset = new BarDataset
            {
                Label = "# hours watched",
                Data = new List<double>(timePerDay.Values),
                BorderWidth = new List<int> { 1 },
                BackgroundColor = new List<string> { "rgb(159, 154, 232)" },
                Type = Enums.ChartType.HorizontalBar
            };
            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;
            chart.Options = new BarOptions
            {
                Responsive = true,
                Title = new Title { Text = "Hours watched per day" }
            };

            return chart;
        }
    }
}