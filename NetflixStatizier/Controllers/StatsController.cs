using ChartJSCore.Models;
using ChartJSCore.Models.Bar;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Stats;
using NetflixStatizier.Stats.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using NetflixStatizier.Data.Repositories.Abstractions;
using NetflixStatizier.Models.InputModels;
using NetflixStatizier.Models.ViewModels;
using Newtonsoft.Json;
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


        public IActionResult Index(NetflixStatsViewModel model)
        {
            return View("Index", model);
        }

        [Route("stats/overview/{identifier:guid}")]
        public async Task<IActionResult> Overview(Guid identifier)
        {
            var viewedItems = await _netflixViewedItemRepository.GetByGuidAsync(identifier);
            if (viewedItems == null)
                return BadRequest($"There are no results saved with the identifier {identifier}");

            var playbacks =
                NetflixViewingHistoryLoader.GetNetflixPlaybacksFromViewingActivity(
                    viewedItems.Select(x => _mapper.Map<Stats.Model.NetflixViewedItem>(x)));

            var calculatedStats = CalculateNetflixStats(playbacks);

            return View("Index", calculatedStats);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Overview(NetflixAccountInputModel model)
        {
            if (!ModelState.IsValid)
                return View("../Home/Index", model);

            var historyLoader = new NetflixViewingHistoryLoader(new NetflixProfile
            {
                AccountEmail = model.NetflixEmail,
                AccountPassword = model.NetflixPassword,
                ProfileName = model.NetflixProfileName
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

        [Route("stats/export/{identifier:guid}")]
        public async Task<IActionResult> Export(ExportInputModel model)
        {
            var viewedItems = await _netflixViewedItemRepository.GetByGuidAsync(model.Identifier);
            if (viewedItems == null)
                return BadRequest($"There are no results saved with the identifier {model.Identifier}");

            if (string.Equals(model.Format, "json"))
            {
                var json = JsonConvert.SerializeObject(viewedItems, Formatting.Indented);

                return File(new MemoryStream(Encoding.UTF8.GetBytes(json)), "application/json",
                    $"TWON-export-{DateTime.Now}.json");
            }

            return BadRequest($"Unknown format: {model.Format}");
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