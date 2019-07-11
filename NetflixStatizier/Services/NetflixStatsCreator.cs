using System;
using System.Collections.Generic;
using System.Linq;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using ChartJSCore.Plugins.Zoom;
using DaHo.Library.Utilities;
using NetflixStatizier.Helper;
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
                .ToDictionary(x => $"{x.Key.Title ?? "Movies"} - {Math.Round(x.Value / 60, 2)}h", y => (double)Math.Round(y.Value / 60, 2));

            var viewedHoursPerDay = statsCalculator.GetViewedMinutesPerDay()
                .OrderBy(x => x.Key)
                .ToDictionary(x => $"{x.Key:dd-MM-yyyy} - {Math.Round(x.Value / 60, 2)}h", y => (double)Math.Round(y.Value / 60, 2));

            var viewedMinutesPerTimeOfDayTemp = statsCalculator.GetViewedMinutesPerTimeOfDay()
                .Select(x =>
                    new KeyValuePair<TimeSpan, double>(x.Key.RoundToNearest(TimeSpan.FromMinutes(15)), (double)Math.Round(x.Value / 60, 2)))
                .GroupBy(x => x.Key)
                .Select(x => new KeyValuePair<TimeSpan, double>(x.Key, x.Sum(y => y.Value)))
                .ToList();

            var viewedMinutesPerTimeOfDay = GetKeyValuePairsForTimeOfDay(viewedMinutesPerTimeOfDayTemp)
                .ToDictionary(x => x.Key, y => y.Value);

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
                HourlyActivityChart = GetHourlyActivityChart(viewedMinutesPerTimeOfDay),
                HighscoreDate = statsCalculator.GetHighscoreDateAndMinutes().date,
                HighcoreTime = Time.FromMinutes(statsCalculator.GetHighscoreDateAndMinutes().minutes)
            };


            return statsModel;
        }

        private static Chart GetTimePerSerieChart(Dictionary<string, double> timePerSerie)
        {
            var chart = new Chart { Type = Enums.ChartType.HorizontalBar };
            var data = new ChartJSCore.Models.Data { Labels = timePerSerie.Keys.ToList() };
            var dataset = new BarDataset
            {
                Label = "# hours watched",
                Data = new List<double>(timePerSerie.Values),
                BorderWidth = new List<int> { 1 },
                BackgroundColor = new List<ChartColor> { ChartColor.FromRgba(159, 154, 232, 0.7) },
                Type = Enums.ChartType.HorizontalBar
            };
            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;
            chart.Options = new ZoomOptions
            {
                Responsive = true,
                Title = new Title { Text = "Hours watched per serie" , Display = true },
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
            var data = new ChartJSCore.Models.Data { Labels = timePerDay.Keys.ToList() };
            var dataset = new BarDataset
            {
                Label = "# hours watched",
                Data = new List<double>(timePerDay.Values),
                BorderWidth = new List<int> { 1 },
                BackgroundColor = new List<ChartColor> { ChartColor.FromRgba(159, 154, 232, 0.7) },
                Type = Enums.ChartType.HorizontalBar
            };
            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;
            chart.Options = new ZoomOptions
            {
                Responsive = true,
                Title = new Title { Text = "Hours watched per day", Display = true },
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

        private static Chart GetHourlyActivityChart(Dictionary<TimeSpan, double> timePerTimeOfDay)
        {
            var labels = timePerTimeOfDay.Keys.Select(x => x.IsFullHour() ? x.ToString("hh':'mm") : "");

            var chart = new Chart { Type = Enums.ChartType.Radar };
            var data = new ChartJSCore.Models.Data { Labels = labels.ToList()};
            var dataset = new RadarDataset
            {
                
                Label = "# hours watched",
                Data = new List<double>(timePerTimeOfDay.Values),
                BorderWidth = 1 ,
                BackgroundColor = ChartColor.FromRgba(159, 154, 232, 0.7),
                Type = Enums.ChartType.Radar
            };
            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;
            chart.Options = new ZoomRadarOptions
            {
                Responsive = true,
                Title = new Title { Text = "Hours watched per time of day", Display = true },
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
                },
                Tooltips = new ToolTip { Enabled = false }
            };

            return chart;
        }

        private IEnumerable<KeyValuePair<TimeSpan, double>> GetKeyValuePairsForTimeOfDay(IReadOnlyCollection<KeyValuePair<TimeSpan, double>> existingKeyValuePairs)
        {
            foreach (var quarterHour in TimeSpanUtilities.GetEveryXMinutesForDay(15))
            {
                if (existingKeyValuePairs.Any(x => x.Key == quarterHour))
                {
                    yield return existingKeyValuePairs.FirstOrDefault(x => x.Key == quarterHour);
                }
                else
                {
                    yield return new KeyValuePair<TimeSpan, double>(quarterHour, 0);
                }
            }
        }
    }
}
