using System;
using System.Collections.Generic;
using System.Linq;
using ChartJSCore.Models;
using ChartJSCore.Plugins.Zoom;
using NetflixStatizier.Models.ViewModels;
using NetflixStatizier.Services.Abstractions;
using NetflixStatizier.Stats;
using NetflixStatizier.Stats.Model;
using Enums = ChartJSCore.Models.Enums;
using Time = NetflixStatizier.Helper.Time;

namespace NetflixStatizier.Services
{
    public class NetflixStatsCreator : INetflixStatsCreator
    {
        public NetflixStatsViewModel GetNetflixStatsViewModel(IEnumerable<NetflixPlayback> viewingHistory)
        {
            var statsCalculator = new NetflixStatsCalculator(viewingHistory);

            var viewedHoursPerSerie = statsCalculator.GetViewedMinutesPerSerie()
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => $"{x.Key} - {Math.Round(x.Value / 60, 2)}h", x => (double)Math.Round(x.Value / 60, 2));

            var viewedHoursPerDay = statsCalculator.GetViewedMinutesPerDay()
                .OrderBy(x => x.Key)
                .ToDictionary(x => $"{x.Key:d} - {Math.Round(x.Value / 60, 2)}h", x => (double)Math.Round(x.Value / 60, 2));

            var viewedHoursPerTimeOfDay = statsCalculator.GetViewedMinutesPerTimeOfDay().OrderBy(x => x.Key).ToList();

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
            chart.Options = new ZoomOptions
            {
                Responsive = true,
                Title = new Title { Text = "Hours watched per serie" },
                ResponsiveAnimationDuration = 500,
                Zoom = new Zoom
                {
                    Enabled = true,
                    Mode = "xy",
                    Speed = 0.2
                },
                Pan = new Pan
                {
                    Enabled = true,
                    Mode = "xy"
                }
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
            chart.Options = new ZoomOptions
            {
                Responsive = true,
                Title = new Title { Text = "Hours watched per day" },
                ResponsiveAnimationDuration = 500,
                Zoom = new Zoom
                {
                    Enabled = true,
                    Mode = "xy",
                    Speed = 0.2
                },
                Pan = new Pan
                {
                    Enabled = true,
                    Mode = "xy"
                }
            };
            
            return chart;
        }
    }
}
