﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetflixStatizier.Helper;
using NetflixStatizier.Models;
using NetflixStatizier.Stats;
using NetflixStatizier.Stats.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace NetflixStatizier.Controllers
{
    public class ResultsController : BaseController
    {
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
                var history = await stats.GetNetflixViewingHistory(model.NetflixProfileName);

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

            var statsModel = new NetflixStatsModel
            {
                TotalViewedTime = Time.FromMinutes((double)statsCalculator.GetTotalViewedMinutes()),
                MoviesViewedTime = Time.FromMinutes((double)statsCalculator.GetMoviesViewedMinutes()),
                SeriesViewedTime = Time.FromMinutes((double)statsCalculator.GetSeriesEpisodesViewedMinutes()),
                MoviesViewedCount = statsCalculator.GetMoviesViewedCount(),
                SeriesEpisodesViewedItemsCount = statsCalculator.GetSeriesEpisodesViewedCount(),
                FirstWatchedMovie = statsCalculator.GetFirstWatchedMovie(),
                FirstWatchedSeriesEpisode = statsCalculator.GetFirstWatchedSeriesEpisode(),
                //ViewedTimePerSerie = statsCalculator.GetViewedMinutesPerSerie().ToDictionary(serieTime => serieTime.Key, serieTime => Time.FromMinutes((double)serieTime.Value))
                ViewedHoursPerSerie = statsCalculator.GetViewedMinutesPerSerie().ToDictionary(serieTime => serieTime.Key, serieTime => serieTime.Value / 60)
            };


            return statsModel;
        }
    }
}