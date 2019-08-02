using System;
using System.Collections.Generic;
using System.Linq;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using ChartJSCore.Plugins.Zoom;
using DaHo.Library.Utilities;
using FlixStats.Models.ViewModels;
using FlixStats.Services.Abstractions;
using FlixStats.Stats;
using FlixStats.Stats.Model;
using Enums = ChartJSCore.Models.Enums;
using Time = FlixStats.Helper.Time;

namespace FlixStats.Services
{
    public class NetflixStatsCreator : INetflixStatsCreator
    {
        public NetflixPlaybacksViewModel GetNetflixPlaybacksViewModel(IEnumerable<NetflixPlayback> viewingHistory)
        {
            var statsCalculator = new NetflixStatsCalculator(viewingHistory);

            return new NetflixPlaybacksViewModel
            {
                Playbacks = viewingHistory.OrderByDescending(x => x.Sort),
                TotalPlaybackTime = Time.FromMinutes(statsCalculator.GetTotalViewedMinutes()),
                PlaybackDate = viewingHistory.FirstOrDefault().PlaybackDateTime.Date,
            };
        }

        public NetflixStatsViewModel GetNetflixStatsViewModel(IEnumerable<NetflixPlayback> viewingHistory)
        {
            var statsCalculator = new NetflixStatsCalculator(viewingHistory);

            var viewedHoursPerSerie = statsCalculator.GetViewedMinutesPerSerie()
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => $"{x.Key.Title ?? "Movies"}", y => (double)Math.Round(y.Value / 60, 2));

            var viewedHoursPerDay = statsCalculator.GetViewedMinutesPerDay()
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key.ToString("yyyy-MM-dd"), y => (double)Math.Round(y.Value / 60, 2));

            var viewedHoursPerWeekDay = statsCalculator.GetViewedMinutesPerWeekDay()
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key.ToString(), y => (double)Math.Round(y.Value / 60, 2));

            var viewedMinutesPerTimeOfDayTemp = statsCalculator.GetViewedMinutesPerTimeOfDay()
                .Select(x =>
                    new KeyValuePair<TimeSpan, double>(x.Key.RoundToNearest(TimeSpan.FromMinutes(15)), (double)Math.Round(x.Value / 60, 2)))
                .GroupBy(x => x.Key)
                .Select(x => new KeyValuePair<TimeSpan, double>(x.Key, x.Sum(y => y.Value)))
                .ToList();

            var totalViewedMinutes = statsCalculator.GetTotalViewedMinutes();
            var viewedPercentagePerCountry = statsCalculator.GetViewedMinutesPerCountry()
                .ToDictionary(x => x.Key, y => (double)Math.Round(y.Value / totalViewedMinutes * 100, 3));

            var viewedMinutesPerTimeOfDay = GetKeyValuePairsForTimeOfDay(viewedMinutesPerTimeOfDayTemp)
                .ToDictionary(x => x.Key, y => y.Value);

            return new NetflixStatsViewModel
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
                WeekDailyActivityChart = GetWeekDailyWatchedChart(viewedHoursPerWeekDay),
                ViewedPercentagePerCountryChart = GetPercentagePerCountryChart(viewedPercentagePerCountry),
                HighscoreDate = statsCalculator.GetHighscoreDateAndMinutes().date,
                HighcoreTime = Time.FromMinutes(statsCalculator.GetHighscoreDateAndMinutes().minutes)
            };
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
                BackgroundColor = new List<ChartColor> { ChartColor.FromRgba(229, 9, 20, 0.8) },
                Type = Enums.ChartType.HorizontalBar
            };
            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;
            chart.Options = new ZoomOptions
            {
                Responsive = true,
                MaintainAspectRatio = false,
                Title = new Title { Text = "Hours watched per serie", Display = true },
                ResponsiveAnimationDuration = 500,
                Zoom = new Zoom
                {
                    Enabled = true,
                    Mode = "xy",
                    Speed = 0.1,
                    RangeMin = new ChartJSCore.Plugins.Zoom.Range { X = 0 }
                },
                Pan = new Pan
                {
                    Enabled = true,
                    Mode = "xy",
                    RangeMin = new ChartJSCore.Plugins.Zoom.Range { X = 0 }
                }
            };

            return chart;
        }

        private static Chart GetTimePerDayChart(Dictionary<string, double> timePerDay)
        {
            var chart = new Chart { Type = Enums.ChartType.HorizontalBar };
            var data = new ChartJSCore.Models.Data
            {
                Labels = timePerDay.Keys.ToList()
            };
            var dataset = new BarDataset
            {
                Label = "# hours watched",
                Data = new List<double>(timePerDay.Values),
                BorderWidth = new List<int> { 1 },
                BackgroundColor = new List<ChartColor> { ChartColor.FromRgba(229, 9, 20, 0.8) },
                Type = Enums.ChartType.HorizontalBar,
            };
            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;
            chart.Options = new ZoomOptions
            {
                Responsive = true,
                MaintainAspectRatio = false,
                Title = new Title { Text = "Hours watched per day", Display = true },
                ResponsiveAnimationDuration = 500,
                Zoom = new Zoom
                {
                    Enabled = true,
                    Mode = "xy",
                    Speed = 0.1,
                    RangeMin = new ChartJSCore.Plugins.Zoom.Range { X = 0 }
                },
                Pan = new Pan
                {
                    Enabled = true,
                    Mode = "xy",
                    RangeMin = new ChartJSCore.Plugins.Zoom.Range { X = 0 }
                }
            };

            return chart;
        }

        private static Chart GetPercentagePerCountryChart(Dictionary<string, double> percentagePerCountry)
        {
            var chart = new Chart { Type = Enums.ChartType.Doughnut };
            var data = new ChartJSCore.Models.Data
            {
                Labels = percentagePerCountry.Keys.ToList()
            };
            var dataset = new PieDataset
            {
                Label = "# hours watched",
                Data = new List<double>(percentagePerCountry.Values),
                BorderWidth = 1,
                BackgroundColor = new List<ChartColor>(),
                Type = Enums.ChartType.Doughnut,
            };

            for (int i = 0; i < percentagePerCountry.Count; i++)
            {
                dataset.BackgroundColor.Add(ChartColor.CreateRandomChartColor(false));
            }

            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;
            chart.Options = new PieOptions
            {
                Responsive = true,
                MaintainAspectRatio = false,
                Title = new Title { Text = "Watched per country (in percentage)", Display = true },
                ResponsiveAnimationDuration = 500
            };

            return chart;
        }

        private static Chart GetHourlyActivityChart(Dictionary<TimeSpan, double> timePerTimeOfDay)
        {
            var labels = timePerTimeOfDay.Keys.Select(x => x.IsFullHour() ? x.ToString("hh':'mm") : "");

            var chart = new Chart { Type = Enums.ChartType.Radar };
            var data = new ChartJSCore.Models.Data { Labels = labels.ToList() };
            var dataset = new RadarDataset
            {

                Label = "# hours watched",
                Data = new List<double>(timePerTimeOfDay.Values),
                BorderWidth = 1,
                BackgroundColor = ChartColor.FromRgba(229, 9, 20, 0.8),
                Type = Enums.ChartType.Radar
            };
            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;
            chart.Options = new RadarOptions
            {
                Responsive = true,
                MaintainAspectRatio = false,
                Title = new Title { Text = "Hours watched per time of day", Display = true },
                ResponsiveAnimationDuration = 500,
                Tooltips = new ToolTip { Enabled = false }
            };

            return chart;
        }

        private static Chart GetWeekDailyWatchedChart(Dictionary<string, double> timePerWeekday)
        {
            var chart = new Chart { Type = Enums.ChartType.Bar };
            var data = new ChartJSCore.Models.Data
            {
                Labels = timePerWeekday.Keys.ToList()
            };
            var dataset = new BarDataset
            {
                Label = "# hours watched",
                Data = new List<double>(timePerWeekday.Values),
                BorderWidth = new List<int> { 1 },
                BackgroundColor = new List<ChartColor> { ChartColor.FromRgba(229, 9, 20, 0.8) },
                Type = Enums.ChartType.Bar,
            };
            data.Datasets = new List<Dataset> { dataset };
            chart.Data = data;
            chart.Options = new Options
            {
                Responsive = true,
                MaintainAspectRatio = false,
                Title = new Title { Text = "Hours watched per weekday", Display = true },
                ResponsiveAnimationDuration = 500,
                Scales = new Scales
                {
                    YAxes = new List<Scale>
                    {
                        new CartesianScale
                        {
                            Ticks = new CartesianLinearTick { BeginAtZero = true }
                        }
                    }
                }
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
