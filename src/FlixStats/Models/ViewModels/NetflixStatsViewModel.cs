using System;
using ChartJSCore.Models;
using FlixStats.Stats.Model;
using Time = DaHo.Library.Utilities.Time;

namespace FlixStats.Models.ViewModels
{
    public class NetflixStatsViewModel
    {
        public Time TotalViewedTime { get; set; }

        public Time SeriesViewedTime { get; set; }

        public Time MoviesViewedTime { get; set; }

        public decimal SeriesEpisodesViewedItemsCount { get; set; }

        public decimal MoviesViewedCount { get; set; }

        public NetflixPlayback FirstWatchedMovie { get; set; }

        public NetflixPlayback FirstWatchedSeriesEpisode { get; set; }

        public Chart ViewedHoursPerDayChart { get; set; }

        public Chart ViewedHoursPerSerieChart { get; set; }

        public Chart HourlyActivityChart { get; set; }

        public Chart WeekDailyActivityChart { get; set; }

        public Chart ViewedPercentagePerCountryChart { get; set; }

        public DateTime HighscoreDate { get; set; }

        public Time HighcoreTime { get; set; }

        public Guid Identifier { get; set; }

        public bool ResultsAreKept { get; set; }

        public bool IsPublishedToLeaderboard { get; set; }

        public DateTime QueryDateTime { get; set; }

        public string ForProfile { get; set; }
    }
}
