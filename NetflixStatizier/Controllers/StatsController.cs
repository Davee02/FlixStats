using ChartJSCore.Models;
using ChartJSCore.Models.Bar;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Stats;
using NetflixStatizier.Stats.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NetflixStatizier.Data.Repositories.Abstractions;
using NetflixStatizier.Models.InputModels;
using NetflixStatizier.Models.ViewModels;
using Enums = ChartJSCore.Models.Enums;
using Time = NetflixStatizier.Helper.Time;

namespace NetflixStatizier.Controllers
{
    public class StatsController : Controller
    {
        private readonly INetflixViewedItemRepository _netflixViewedItemRepository;
        private readonly IMapper _mapper;


        public StatsController(INetflixViewedItemRepository netflixViewedItemRepository, IMapper mapper)
        {
            _netflixViewedItemRepository = netflixViewedItemRepository;
            _mapper = mapper;
        }


        public IActionResult Index(NetflixStatsViewModel netflixStatsViewModel)
        {
            return View("Index", netflixStatsViewModel);
        }

        public async Task<IActionResult> Overview(Guid id)
        {
            var viewedItems = await _netflixViewedItemRepository.GetByGuidAsync(id);
            var playbacks =
                NetflixViewingHistoryLoader.GetNetflixPlaybacksFromViewingActivity(
                    viewedItems.Select(x => _mapper.Map<Stats.Model.NetflixViewedItem>(x)));

            var calculatedStats = CalculateNetflixStats(playbacks);

            return View("Index", calculatedStats);
        }

        [HttpPost]
        public async Task<IActionResult> Overview(NetflixAccountInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return View("../Home/Index", inputModel);

            var historyLoader = new NetflixViewingHistoryLoader(new NetflixProfile
            {
                AccountEmail = inputModel.NetflixEmail,
                AccountPassword = inputModel.NetflixPassword,
                ProfileName = inputModel.NetflixProfileName
            });

            var viewedItems = await historyLoader.LoadNetflixViewedItemsAsync();

            var identificationGuid = Guid.NewGuid();
            var mappedItems = new List<Models.EntityFrameworkModels.NetflixViewedItem>();
            foreach (var netflixViewedItem in viewedItems)
            {
                var mapped = _mapper.Map<Models.EntityFrameworkModels.NetflixViewedItem>(netflixViewedItem);
                mapped.Identifier = identificationGuid;

                mappedItems.Add(mapped);
            }

            await _netflixViewedItemRepository.CreateManyAsync(mappedItems);

            return RedirectToAction("Overview", new { id = identificationGuid });
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