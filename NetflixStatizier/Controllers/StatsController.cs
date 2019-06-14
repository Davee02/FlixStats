using ChartJSCore.Models;
using ChartJSCore.Models.Bar;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Stats;
using NetflixStatizier.Stats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixStatizier.Data.Repositories.Abstractions;
using NetflixStatizier.Models.InputModels;
using NetflixStatizier.Models.ViewModels;
using NetflixStatizier.Services;
using RestEase;
using Enums = ChartJSCore.Models.Enums;
using Time = NetflixStatizier.Helper.Time;

namespace NetflixStatizier.Controllers
{
    public class StatsController : Controller
    {
        private readonly INetflixViewedItemRepository _netflixViewedItemRepository;

        public StatsController(INetflixViewedItemRepository netflixViewedItemRepository)
        {
            _netflixViewedItemRepository = netflixViewedItemRepository;
        }

        public IActionResult Index(NetflixStatsViewModel netflixStatsViewModel)
        {
            return View("Index", netflixStatsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Overview(NetflixAccountInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return View("../Home/Index", inputModel);

            var netflixApi = RestClient.For<INetflixApi>("https://localhost:5005/api");
            var history = await netflixApi.GetNetflixViewingHistory(new NetflixProfile
            {
                AccountEmail = inputModel.NetflixEmail,
                AccountPassword = inputModel.NetflixPassword,
                ProfileName = inputModel.NetflixProfileName
            });

            var calculatedStats = CalculateNetflixStats(history);

            return Index(calculatedStats);
        }


        private static NetflixStatsViewModel CalculateNetflixStats(IEnumerable<NetflixPlayback> viewingHistory)
        {
            var statsCalculator = new NetflixStatsCalculator(viewingHistory);

            var viewedHoursPerSerie = statsCalculator.GetViewedMinutesPerSerie()
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => $"{x.Key} - {Math.Round(x.Value / 60, 2)}h", x => (double)Math.Round(x.Value / 60, 2));

            var viewedHoursPerDay = statsCalculator.GetViewedMinutesPerDay()
                .OrderBy(x => x.Key)
                .ToDictionary(x => $"{x.Key:d} - {Math.Round(x.Value / 60, 2)}h", x => (double)Math.Round(x.Value / 60, 2));


            var statsModel = new NetflixStatsViewModel
            {
                TotalViewedTime = Time.FromMinutes(statsCalculator.GetTotalViewedMinutes()),
                MoviesViewedTime = Time.FromMinutes(statsCalculator.GetMoviesViewedMinutes()),
                SeriesViewedTime = Time.FromMinutes(statsCalculator.GetSeriesEpisodesViewedMinutes()),
                MoviesViewedCount = statsCalculator.GetMoviesViewedCount(),
                SeriesEpisodesViewedItemsCount = statsCalculator.GetSeriesEpisodesViewedCount(),
                FirstWatchedMovie = statsCalculator.GetFirstWatchedMovie(),
                FirstWatchedSeriesEpisode = statsCalculator.GetFirstWatchedSeriesEpisode(),
                ViewedHoursPerSerieChart = GetTimePerSerieChart(viewedHoursPerSerie),
                ViewedHoursPerDayChart = GetTimePerDayChart(viewedHoursPerDay),
                HighscoreDate = statsCalculator.GetHighscoreDateAndMinutes().date,
                HighcoreTime = Time.FromMinutes(statsCalculator.GetHighscoreDateAndMinutes().minutes)
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