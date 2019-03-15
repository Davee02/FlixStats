using ChartJSCore.Models;
using NetflixStatizier.Stats.Model;
using Time = NetflixStatizier.Helper.Time;

namespace NetflixStatizier.Models
{
    public class NetflixStatsModel
    {
        public Time TotalViewedTime { get; set; }

        public Time SeriesViewedTime { get; set; }

        public Time MoviesViewedTime { get; set; }

        public decimal SeriesEpisodesViewedItemsCount { get; set; }

        public decimal MoviesViewedCount { get; set; }

        public NetflixPlayback FirstWatchedMovie { get; set; }

        public NetflixPlayback FirstWatchedSeriesEpisode { get; set; }

        public Chart ViewedHoursPerDayChart { get; set; }

        public Chart ViewedHoursPerSerieChart{ get; set; }
    }
}
